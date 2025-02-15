# mAnswer
A small .NET C# middleware to automate ProxMox installation using answer file generation based on the host mac address.

##How to Use
To use this service, send a POST request to the /answer endpoint with a JSON payload. The JSON payload should include network interface data. When automated installation is ongoing, Proxmox installer does the POST request automatically.

##Example POST Request
curl -X POST http://host:port/answer -H "Content-Type: application/json" -d @data.json
The data.json file contains host information including network interface data:

{
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
}
##Example answer file
Answer files are located in the answerFiles folder at the webapp root. The filename is based on the MAC address: 01_23_45_67_89_ab.toml. Example format of the file is:

[global]
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
filesystem = "ext4"
