using Zeroconf;

namespace PrintZPL.Core.Services;
public class MdnsPrinterDiscoveryService : IPrinterDiscoveryService
{
    private static readonly string[] ServiceTypes = { "_printer._tcp.local." };

    public async Task<IEnumerable<PrinterInfo>> DiscoverAsync(CancellationToken ct = default)
    {
        var responses = await ZeroconfResolver.ResolveAsync(
            ServiceTypes,
            scanTime: TimeSpan.FromSeconds(3),
            callback: null,
            cancellationToken: ct);

        var printers = new List<PrinterInfo>();

        foreach (var resp in responses)
        {
            if (!resp.Services.TryGetValue(ServiceTypes[0], out var svc))
                continue;

            string? model = null;
            if (svc.Properties != null)
            {
                foreach (var txtRecord in svc.Properties)
                {
                    if (txtRecord.TryGetValue("ty", out var val) && !string.IsNullOrEmpty(val))
                    {
                        model = val;
                        break;
                    }
                }
            }

            printers.Add(new PrinterInfo(
                Name: resp.DisplayName,
                IpAddress: resp.IPAddress,
                Port: svc.Port,
                Model: model
            ));
        }

        return printers;
    }
}
