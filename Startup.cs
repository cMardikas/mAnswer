using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tomlyn;

public class Startup
{
    public static readonly string DEFAULT_ANSWER_FILE_PATH = "./defaultAnswer.toml";
    public static readonly string ANSWER_FILE_DIR = "./answerFiles/";

    public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
			// Serve the landing page
            endpoints.MapGet("/", async context =>
            {
                var landingPage = await File.ReadAllTextAsync("index.html");
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(landingPage);
            });
			
            endpoints.MapPost("/answer", async context =>
            {
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

                try
                {
                    var requestData = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody);

                    if (requestData != null)
                    {
                        logger.LogInformation($"Request data for peer '{context.Connection.RemoteIpAddress}':\n{JsonConvert.SerializeObject(requestData, Formatting.Indented)}");

                        var answer = CreateAnswer(requestData, logger);

                        logger.LogInformation($"Answer file for peer '{context.Connection.RemoteIpAddress}':\n{answer}");
                        await context.Response.WriteAsync(answer);
                    }
                    else
                    {
                        logger.LogError("Request data is null");
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Internal Server Error: Request data is null");
                    }
                }
                catch (JsonException e)
                {
                    logger.LogError($"Failed to parse request contents: {e}");
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync($"Internal Server Error: failed to parse request contents: {e}");
                }
                catch (Exception e)
                {
                    logger.LogError($"Failed to create answer: {e}");
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync($"Internal Server Error: {e}");
                }
            });
        });
    }

    private static string CreateAnswer(Dictionary<string, object> requestData, ILogger logger)
    {
        var answer = ParseTomlFile(DEFAULT_ANSWER_FILE_PATH);

        if (requestData.TryGetValue("network_interfaces", out var networkInterfaces))
        {
            foreach (var nic in (JArray)networkInterfaces)
            {
                var nicDict = nic.ToObject<Dictionary<string, string>>();
                if (nicDict != null && nicDict.ContainsKey("mac"))
                {
                    var answerMac = LookupAnswerForMac(nicDict["mac"], logger);
                    if (answerMac != null)
                    {
                        answer = answerMac;
                    }
                }
            }
        }

        return Toml.FromModel(answer);
    }

    private static Dictionary<string, object>? LookupAnswerForMac(string mac, ILogger logger)
    {
		var macSanitized = mac.Replace(":", "_").ToLower(); // Replace colons with underscores and convert to lowercase
        var answerFiles = Directory.GetFiles(ANSWER_FILE_DIR, "*.toml");

        logger.LogInformation($"Looking up MAC address: {macSanitized}");

        foreach (var filename in answerFiles)
        {
            var fileNameLower = Path.GetFileNameWithoutExtension(filename).ToLower();
            logger.LogInformation($"Comparing with file: {fileNameLower}");

            if (fileNameLower.Contains(macSanitized))
            {
                logger.LogInformation($"Match found with file: {filename}");
                var tomlData = ParseTomlFile(filename);
                return tomlData;
            }
        }

        logger.LogInformation($"No match found for MAC address: {macSanitized}");
        return null;
    }

    private static Dictionary<string, object> ParseTomlFile(string path)
    {
        var tomlContent = File.ReadAllText(path);
        return Toml.Parse(tomlContent).ToModel<Dictionary<string, object>>();
    }

    private static void AssertFileExists(string path)
    {
        if (!File.Exists(path))
        {
            throw new Exception($"File '{path}' does not exist");
        }
    }
}
