namespace PrintZPL.Core.Services;

public interface IPrinterDiscoveryService
{
    /// <summary>
    /// Discover printers on the local network.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of discovered printers.</returns>
    Task<IEnumerable<PrinterInfo>> DiscoverAsync(CancellationToken ct = default);
}

public record PrinterInfo(
    string Name,
    string IpAddress,
    int Port,
    string? Model
);


