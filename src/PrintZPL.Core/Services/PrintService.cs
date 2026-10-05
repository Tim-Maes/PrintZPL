using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PrintZPL.Core.Services;

public sealed class PrintService : IPrintService
{
    private readonly ILogger<PrintService> _logger;
    private readonly ITemplateService _templateService;
    private readonly IPrinterAddressPolicy _printerAddressPolicy;

    public PrintService(
        ILogger<PrintService> logger,
        ITemplateService templateService,
        IPrinterAddressPolicy printerAddressPolicy)
    {
        _logger = logger;
        _templateService = templateService;
        _printerAddressPolicy = printerAddressPolicy;
    }

    public async Task PrintZPL(string zplString, string printerIpAddress, int port, Dictionary<string, string>? data, string delimiter, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Printing ZPL template to {IpAddress}:{Port}", printerIpAddress, port);

        if (string.IsNullOrWhiteSpace(zplString))
            throw new ArgumentException("ZPL data is required.", nameof(zplString));
        if (!IPAddress.TryParse(printerIpAddress, out var printerAddress))
            throw new ArgumentException("Printer address must be an IP address.", nameof(printerIpAddress));
        if (!_printerAddressPolicy.IsAllowed(printerAddress))
            throw new ArgumentException("Printer address is not in the configured allowlist.", nameof(printerIpAddress));
        if (port is < 1 or > 65535)
            throw new ArgumentOutOfRangeException(nameof(port), "Printer port must be between 1 and 65535.");

        var template = zplString;

        if (data is not null && data.Count > 0)
        {
            if (data.Count > 100 || data.Keys.Any(key => key.Length > 256))
                throw new ArgumentException("Template data is limited to 100 entries with keys up to 256 characters.", nameof(data));

            template = _templateService.PopulateZplTemplate(data, zplString, delimiter);
        }

        if (!template.EndsWith("\n"))
        {
            template += "\n";
        }

        if (Encoding.UTF8.GetByteCount(template) > 262144)
            throw new ArgumentException("ZPL data exceeds the 256 KB limit.", nameof(zplString));

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(5));

        try
        {
            using var client = new TcpClient();

            _logger.LogDebug("Connecting to printer at {IpAddress}:{Port}", printerIpAddress, port);

            await client.ConnectAsync(printerIpAddress, port, timeout.Token);

            _logger.LogDebug("Connected to printer, sending ZPL data");

            using var stream = client.GetStream();

            var bytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false).GetBytes(template);
            await stream.WriteAsync(bytes, timeout.Token);
            await stream.FlushAsync(timeout.Token);

            _logger.LogInformation("ZPL sent to printer successfully. Data length: {Length} characters", template.Length);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Timed out while printing to {IpAddress}:{Port}", printerIpAddress, port);
            throw new TimeoutException("Timed out while communicating with the printer.", ex);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (SocketException ex)
        {
            _logger.LogError(ex, "Socket error while connecting to printer {IpAddress}:{Port} - {Message}",
                printerIpAddress, port, ex.Message);
            throw new InvalidOperationException("Failed to communicate with the printer.", ex);
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
    Task PrintZPL(string zplString, string printerIpAddress, int port, Dictionary<string, string>? data, string delimiter, CancellationToken cancellationToken = default);
}
