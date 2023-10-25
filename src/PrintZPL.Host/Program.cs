using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PrintZPL.Host;
using System.Diagnostics;
using System.ServiceProcess;

class Program
{
    public static void Main(string[] args)
    {
        bool isService = !(Debugger.IsAttached || args.Contains("--console"));

        if (isService)
        {
            var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            Directory.SetCurrentDirectory(pathToContentRoot);

        }

        IWebHost host = CreateWebHostBuilder(args.Where(arg => arg != "--console").ToArray(), isService).Build();

        if (isService)
        {
            using var webHostService = new InternalWebHostingService(host);
            ServiceBase.Run(webHostService);
        }
        else
        {
            host.Run();
        }
    }

    private static IWebHostBuilder CreateWebHostBuilder(string[] args, bool isService)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        return WebHost.CreateDefaultBuilder(args)
                 .ConfigureLogging(logging =>
                 {
                     logging
                         .ClearProviders()
                         .AddFilter("Microsoft", LogLevel.Warning)
                         .AddFilter("Microsoft.AspNetCore.Mvc.Internal", LogLevel.Warning)
                         .AddFilter("System", LogLevel.Warning)
                         .AddConsole();

                     if (isService)
                         logging.AddEventLog(settings =>
                         {
                             settings.SourceName = "PrintZPL";
                         });
                 })
             .UseStartup<Startup>()
                .UseUrls(configuration.GetValue<string>("Host:Urls"))
                .UseKestrel()
                .UseConfiguration(configuration); ;
    }
}