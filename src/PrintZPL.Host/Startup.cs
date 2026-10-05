using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrintZPL.Core;

namespace PrintZPL.Host;

internal class Startup
{
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public Startup(Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var allowedAddresses = _configuration.GetSection("Printers:AllowedAddresses")
            .GetChildren()
            .Select(section => section.Value ?? string.Empty)
            .ToArray();
        services.AddCore(allowedAddresses);
        services.AddRouting();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseMiddleware<ApiKeyMiddleware>();
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
