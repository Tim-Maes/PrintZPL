using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PrintZPL.Host;

internal class InternalWebHostingService : WebHostService
{
    private readonly ILogger _logger;

    public InternalWebHostingService(IWebHost host) : base(host)
    {
        _logger = host.Services.GetRequiredService<ILogger<InternalWebHostingService>>();
    }

    protected override void OnStarting(string[] args)
    {
        _logger.LogInformation("PrintZPL service is starting...");
        base.OnStarting(args);
    }

    protected override void OnStarted()
    {
        _logger.LogInformation("PrintZPL service has started.");
        base.OnStarted();
    }

    protected override void OnStopping()
    {
        _logger.LogInformation("PrintZPL service is Stopping...");
        base.OnStopping();
    }

    protected override void OnStopped()
    {
        _logger.LogInformation("PrintZPL service has stopped.");
        base.OnStopped();
    }
}
