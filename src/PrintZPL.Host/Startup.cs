using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrintZPL.Core;

namespace PrintZPL.Host;

internal class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCore();
        services.AddRouting();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
