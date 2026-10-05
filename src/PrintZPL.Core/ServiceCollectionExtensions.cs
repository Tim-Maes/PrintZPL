using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PrintZPL.Core.Services;

namespace PrintZPL.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IEnumerable<string> allowedPrinterAddresses)
    {
        services.TryAddScoped<IPrintService, PrintService>();
        services.TryAddScoped<ITemplateService, TemplateService>();

        var allowedAddresses = allowedPrinterAddresses
            .Select(address => System.Net.IPAddress.TryParse(address, out var parsed)
                ? parsed
                : throw new InvalidOperationException($"Invalid printer IP address in configuration: {address}"))
            .ToHashSet();
        services.AddSingleton<IPrinterAddressPolicy>(new ConfiguredPrinterAddressPolicy(allowedAddresses));
        services.AddSingleton<IPrinterDiscoveryService, MdnsPrinterDiscoveryService>();
        return services;
    }

    private sealed class ConfiguredPrinterAddressPolicy(HashSet<System.Net.IPAddress> allowedAddresses) : IPrinterAddressPolicy
    {
        public bool IsAllowed(System.Net.IPAddress address) => allowedAddresses.Contains(address);
    }
}
