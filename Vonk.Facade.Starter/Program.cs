using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Vonk.Facade.Starter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
             .ConfigureLogging((hostingContext, logging) =>
             {
                 logging.SetMinimumLevel(LogLevel.Trace);
             })
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                var hostingEnv = hostContext.HostingEnvironment;
                var runningEnv = hostingEnv?.EnvironmentName?.ToLower() ?? "release";
                config.Sources.Clear(); // Clear default sources

                config
                    .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile(path: "appsettings.json", reloadOnChange: true, optional: true);
            })
            .UseStartup<Startup>()
                .Build();
    }
}
