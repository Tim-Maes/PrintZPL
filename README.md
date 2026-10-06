# PrintZPL

## Description

This service allows you to discover Zebra printers and send/print ZPL templates by using HTTP POST requests.

The HTTP API requires an API key. Configure a random secret of at least 32 characters before starting the service. For example, set `Security__ApiKey` in the process environment (Windows PowerShell: `$env:Security__ApiKey = "<random-secret>"`; Linux/macOS: `export Security__ApiKey="<random-secret>"`). Requests must include it in the `X-API-Key` header. The service fails closed with HTTP 503 when no valid key is configured.

Configure `Printers:AllowedAddresses` with the exact printer IP addresses that this service may contact. The list is empty by default, so printing is denied until an allowlist is configured. For example, in `appsettings.json`:

```json
{
  "Printers": {
    "AllowedAddresses": [ "192.168.1.30" ]
  }
}
```

Keep the service on a trusted network, use firewall rules to restrict callers, and do not expose the HTTP endpoint directly to the public internet. Print requests are limited to 256 KB of expanded ZPL; batches accept at most 50 items. Printer connections have a five-second timeout.

## Installation

### Download and run as service

- [PrintZPL-win-x64](https://github.com/Tim-Maes/PrintZPL/actions/runs/37311999616/artifacts/11346177294) for Windows
- [PrintZPL-linux-x64](https://github.com/Tim-Maes/PrintZPL/actions/runs/37311999616/artifacts/11346741178) for Linux
- [PrintZPL-osx-x65](https://github.com/Tim-Maes/PrintZPL/actions/runs/37311999616/artifacts/11346202331) for MaxOS

### Running as a Service

1. Create the service:

```powershell
sc create MyServiceName ^
    binPath= "C:\Full\Path\To\PrintZpl.exe" ^
    DisplayName= "PrintZpl" ^
    start= auto
```

2. Start the service

```powershell
sc start PrintZpl
```

## The API

### Discovering printers

You can send a GET request to `http://localhost:9001/printers` with an `X-API-Key` header.
Example response:

```json
[
    {
        "name": "TestPrinter",
        "ipAddress": "192.168.1.30",
        "port": 9100,
        "model": "ZebraTest"
    }
]
```

### Print ZPL Labels

Send a POST request to `http://localhost:9001/print/from-zpl` with an `X-API-Key` header.

Using these parameters you can send a ZPL template to a printer:

```json
{
    "ZPL": "^XA^FO50,50^ADN,36,20^FDHello, world!!^FS^XZ",
    "IpAddress": "192.168.1.30",
    "Port": 9100
}
```

The port defaults to `6101` when omitted. Set it explicitly to the printer's raw TCP port (commonly `9100`).
### Print ZPL Label with data

You can also send data parameters to process a template that has placeholders for data and specify a delimiter.

For example, if you use the `$` delimiter in your ZPL template, you can send the following request:

```json
{
    "ZPL": "^XA^FO50,50^ADN,36,20^FD$Greeting$, $Name$!^FS^XZ",
    "IpAddress": "192.168.1.30",
    "Port": 9100,
    "Data": {
        "Greeting": "Hello",
        "Name": "World"
    },
    "Delimiter": "$"
}
```

`$Greeting$` and `$Name$` will be replaced by `Hello` and `World` respectively.

### Print ZPL Labels in batch

Url: `http://localhost:9001/batch-print/from-zpl` (include the `X-API-Key` header)

You can send a batch of ZPL templates to a printer by using the following request:

```json
{
    "PrintRequests":
    [
        {
            "ZPL": "^XA^FO50,50^ADN,36,20^FDHello, $Name$!^FS^XZ",
            "IpAddress": "192.168.1.30",
            "Port": 9100,
            "Data": {
                "Name": "World",
                "Name2": "OtherValue"
            },
            "Delimiter": "$"
        },
        {
            "ZPL": "^XA^FO50,50^ADN,36,20^FDHello, $Name$!^FS^XZ",
            "IpAddress": "192.168.1.30",
            "Port": 9100,
            "Data": {
                "Name": "World",
                "Name2": "OtherValue"
            },
            "Delimiter": "$"
        }
    ]
}
```

## The code

### Building the project

### Prerequisites

- .NET 8 SDK

```bash
- dotnet build
```

### Running the project

```bash
dotnet run
```

This will start a web server on port 9001.

### Pack this repo code as executable

Run this command in the project folder

**Windows**
```bash
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true --self-contained true
```
**Linux** 

```bash
dotnet publish -r linux-x64 -c Release /p:PublishSingleFile=true --self-contained true
```
**MacOs**

```bash
dotnet publish -r osx-x64 -c Release /p:PublishSingleFile=true --self-contained true
```

You'll find the output .exe in `bin\Release\net8.0\win-x64\publish` (or the corresponding folder for your target platform).


