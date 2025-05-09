using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PrintZPL.Core.Services;

namespace PrintZPL.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.TryAddScoped<IPrintService, PrintService>();
        services.TryAddScoped<ITemplateService, TemplateService>();

        services.AddSingleton<IPrinterDiscoveryService, MdnsPrinterDiscoveryService>();
        return services;
    }
}
