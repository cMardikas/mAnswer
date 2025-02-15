public class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .Build();

        AssertDirectoryExists(Startup.ANSWER_FILE_DIR);
        AssertFileExists(Startup.DEFAULT_ANSWER_FILE_PATH);

        host.Run();
    }

    private static void AssertFileExists(string path)
    {
        if (!File.Exists(path))
        {
            throw new Exception($"File '{path}' does not exist");
        }
    }

    private static void AssertDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new Exception($"Directory '{path}' does not exist");
        }
    }
}
