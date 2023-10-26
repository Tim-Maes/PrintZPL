using Microsoft.Extensions.Logging;
using PrintZPL.Core.Exceptions;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.RegularExpressions;

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

        if (!await IsKnownIPAddress(printerIpAddress))
            throw new PrinterNotFoundException(printerIpAddress);

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

    public async Task<bool> IsKnownIPAddress(string ipAddress)
    {
        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "arp -a",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        process.WaitForExit();

        var regex = new Regex(ipAddress);
        return regex.IsMatch(output);
    }
}

public interface IPrintService
{
    Task PrintZPL(string zplString, string printerIpAddress, int port);
}