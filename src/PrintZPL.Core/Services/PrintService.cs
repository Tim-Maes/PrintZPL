using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text;

namespace PrintZPL.Core.Services;

public sealed class PrintService : IPrintService
{
    private readonly ILogger<PrintService> _logger;
    private readonly ITemplateService _templateService;

    public PrintService(
        ILogger<PrintService> logger,
        ITemplateService templateService)
    {
        _logger = logger;
        _templateService = templateService;
    }

    public async Task PrintZPL(string zplString, string printerIpAddress, int port, Dictionary<string, string> data, string delimiter)
    {
        _logger.LogInformation("Printing ZPL template to {IpAddress}:{Port}", printerIpAddress, port);

        if (string.IsNullOrWhiteSpace(zplString))
            throw new ArgumentNullException(nameof(zplString));

        var template = zplString;

        if (data is not null && data.Count > 0)
        {
            template = _templateService.PopulateZplTemplate(data, zplString, delimiter);
        }

        if (!template.EndsWith("\n"))
        {
            template += "\n";
        }

        try
        {
            using var client = new TcpClient();
            
            client.ReceiveTimeout = 5000;
            client.SendTimeout = 5000;
            
            _logger.LogDebug("Connecting to printer at {IpAddress}:{Port}", printerIpAddress, port);
            
            await client.ConnectAsync(printerIpAddress, port);
            
            _logger.LogDebug("Connected to printer, sending ZPL data");
            
            using var stream = client.GetStream();
            
            using var writer = new StreamWriter(stream, Encoding.UTF8, bufferSize: 1024, leaveOpen: false)
            {
                AutoFlush = false
            };
            
            await writer.WriteAsync(template);
            await writer.FlushAsync();
            
            await stream.FlushAsync();
            
            _logger.LogInformation("ZPL sent to printer successfully. Data length: {Length} characters", template.Length);
        }
        catch (SocketException ex)
        {
            _logger.LogError(ex, "Socket error while connecting to printer {IpAddress}:{Port} - {Message}", 
                printerIpAddress, port, ex.Message);
            throw new InvalidOperationException($"Failed to connect to printer at {printerIpAddress}:{port}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while printing ZPL to {IpAddress}:{Port}", printerIpAddress, port);
            throw;
        }
    }
}

public interface IPrintService
{
    Task PrintZPL(string zplString, string printerIpAddress, int port, Dictionary<string, string> data, string delimiter);
}