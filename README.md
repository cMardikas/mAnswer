# mAnswer
A small .NET C# middleware to automate ProxMox installation using answer file generation based on the deployed host mac address.

## How-to
<p>To use this service, send a <code>POST</code> request to the <code>/answer</code> endpoint with a JSON payload. The JSON payload should include network interface data. When automated installation is ongoing, Proxmox installer does the POST request automatically.</p>
        <h3>Example POST Request</h3>
        <pre><code>curl -X POST http://host:port/answer -H "Content-Type: application/json" -d @data.json</code></pre>
        <p>The <code>data.json</code> file is a simulated ProxMox POST request wich contains host information including network interface mac address:</p>
        <pre><code>{
    "product": {
        "fullname": "Proxmox VE",
        "product": "pve",
        "enable_btrfs": true
    },
    "iso": {
        "release": "8.2",
        "isorelease": "2"
    },
    "dmi": {
        "system": {
            "name": "Standard PC (i440FX + PIIX, 1996)",
            "serial": "",
            "uuid": "00000000-0000-0000-0000-000000000000",
            "sku": ""
        },
        "baseboard": {},
        "chassis": {
            "asset_tag": "",
            "serial": ""
        }
    },
    "network_interfaces": [
        {
            "link": "ens18",
            "mac": "01:23:45:67:89:ab"
        }
    ]
}</code></pre>
        <h3>Example answer file</h3>
        <p>Answer files are located in the <code>answerFiles</code> folder at the webapp root. The filename is based on the MAC address: <code>01_23_45_67_89_ab.toml</code>. Example format of the file is:</p>
        <pre><code>[global]
keyboard = "en-us"
country = "us"
fqdn = "pveauto.install"
mailto = "mail@nobody"
timezone = "Estonia/Tallinn"
root_password = "123456"
root_ssh_keys = []

[network]
source = "from-dhcp"

[disk-setup]
filesystem = "ext4"</code></pre>
