# PrintZPL

## Usage

Run the console application and send a POST request to the controller endpoint using Postman. 

Or you can just use `CURL`

```curl
curl -X POST "http://localhost:9001/print/from-zpl" \
     -H "Content-Type: multipart/form-data" \
     -F "ZPL=ZPLTEMPLATE" \
     -F "IpAddress=ZEBRAIPADDRESS" \
     -F "Port=PORT"   // Mostly 6101
```

