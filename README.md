# PrintZPL

## Description
This service allows you to send ZPL templates to a Zebra printer by using HTTP POST requests.

## Installation
### Prerequisites
- .NET 6 SDK

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

### Building the project

```bash
dotnet build
```

### Running the project
```bash
dotnet run
```

## API
### Print ZPL Label

You try sending a request with Postman

- **URL**: `/print/from-zpl`
- **Method**: `POST`
- **Data params**:
```json
{
  "ZPL": "string",
  "IpAddress": "string",
  "Port": "int"
}
```
Or test with `curl`

```curl
curl -X POST "http://localhost:9001/print/from-zpl" \
     -H "Content-Type: multipart/form-data" \
     -F "ZPL=ZPLTEMPLATE" \
     -F "IpAddress=ZEBRAIPADDRESS" \
     -F "Port=PORT"   // Mostly 6101
```

