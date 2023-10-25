# PrintZPL

## Description
This service allows you to print ZPL labels to a Zebra printer by sending HTTP POST requests.

## Installation
### Prerequisites
- .NET 6 SDK

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

- **URL**: /print/from-zpl
- **Method**: POST
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

