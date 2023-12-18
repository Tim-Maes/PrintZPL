using Microsoft.Extensions.Logging;
using System.Net.Sockets;

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
        _logger.LogInformation($"Printing ZPL template");

        if (string.IsNullOrEmpty(zplString))
            throw new ArgumentNullException(nameof(zplString));

        var template = zplString;

        if (data is not null)
        {
            template =  _templateService.PopulateZplTemplate(data, zplString, delimiter);
        }

        try
        {
            using (TcpClient client = new TcpClient(printerIpAddress, port))
            using (NetworkStream stream = client.GetStream())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(template);
                await writer.FlushAsync();
            }

            _logger.LogInformation("ZPL sent to printer successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}");
        }
    }
}

public interface IPrintService
{
    Task PrintZPL(string zplString, string printerIpAddress, int port, Dictionary<string, string> data, string delimiter);
}