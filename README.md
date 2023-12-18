# PrintZPL

## Description
This service allows you to send ZPL templates to a Zebra printer by using HTTP POST requests.

## API

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

You try sending a request to `http://localhost:9001/print/from-zpl`

For installation as a executable application, see instructions below.

### Print ZPL Label

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


## Installation

### Pack as executable

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
