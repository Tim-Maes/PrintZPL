using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace PrintZPL.Core.Services;

public sealed class PrintService : IPrintService
{
    private readonly ILogger<PrintService> _logger;

    public PrintService(
        ILogger<PrintService> logger)
    {
        _logger = logger;
    }

    public async Task PrintZPL(string zplString, string printerIpAddress, int port)
    {
        _logger.LogInformation($"Printing ZPL template");

        if (string.IsNullOrEmpty(zplString))
            throw new ArgumentNullException(nameof(zplString));

        try
        {
            using (TcpClient client = new TcpClient(printerIpAddress, port))
            using (NetworkStream stream = client.GetStream())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(zplString);
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
    Task PrintZPL(string zplString, string printerIpAddress, int port);
}