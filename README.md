# PrintZPL

## Description
PrintZPL is a lightweight web service to discover Zebra printers via mDNS and send ZPL templates over TCP using simple HTTP calls.

## Installation

### Pre-reqs

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Download and run as service

- [PrintZPL-win-x64](https://github.com/Tim-Maes/PrintZPL/actions/runs/14935544402/artifacts/3096169978) for Windows
- [PrintZPL-linux-x64](https://github.com/Tim-Maes/PrintZPL/actions/runs/14935544402/artifacts/3096169686) for Linux
- [PrintZPL-osx-x65](https://github.com/Tim-Maes/PrintZPL/actions/runs/14935544402/artifacts/3096170002) for macOS

## The API

### Discovering printers

You can send a GET request to `http//localhost:9001/printers`
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

You try sending a request to `http://localhost:9001/print/from-zpl`

Using these parameters you can send a ZPL template to a printer:

```json
{
    "ZPL": "^XA^FO50,50^ADN,36,20^FDHello, world!!^FS^XZ",
    "IpAddress": "0.0.0.0.0",
    "Port": "6101"
}
```

### Print ZPL Label with data

You can also send data parameters to process a template that has placeholders for data and specify a delimiter.

For example, if you use the `$` delimiter in your ZPL template, you can send the following request:

```json
{
    "ZPL": "^XA^FO50,50^ADN,36,20^FD$Greeting$, $Name$!^FS^XZ",
    "IpAddress": "0.0.0.0.0",
    "Port": "6101",
    "Data": {
        "Greeting": "Hello",
        "Name": "World"
    },
    "Delimiter": "$"
}
```

`$Greeting]$` and `$Name$` will be replaced by `Hello` and `World` respectively.

### Print ZPL Labels in batch

Url: `http://localhost:9001/batch-print/from-zpl`
You can send a batch of ZPL templates to a printer by using the following request:

```json
{
    "PrintRequests":
    [
        {
            "ZPL": "^XA^FO50,50^ADN,36,20^FDHello, $Name$!^FS^XZ",
            "IpAddress": "0.0.0.0.0",
            "Port": "6101",
            "Data": {
                "Name": "World",
                "Name2": "OtherValue"
            },
            "Delimiter": "$"
        },
        {
            "ZPL": "^XA^FO50,50^ADN,36,20^FDHello, $Name$!^FS^XZ",
            "IpAddress": "0.0.0.0.0",
            "Port": "6101",
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
- .NET 6 SDK
```bash
dotnet build
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

You'll find the output .exe in `bin\Release\net6.0\win-x64\publish`


