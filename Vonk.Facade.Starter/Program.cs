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
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                var hostingEnv = hostContext.HostingEnvironment;
                var runningEnv = hostingEnv?.EnvironmentName?.ToLower() ?? "release";
                config.Sources.Clear(); // Clear default sources

                config
                    .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile(path: "appsettings.json", reloadOnChange: true, optional: true)
                    .AddJsonFile(path: "appsettings.development.json", reloadOnChange: true, optional: true) //Load debug specific settings. 
                    .AddJsonFile(path: "appsettings.instance.json", reloadOnChange: true, optional: true); //Load instance specific settings. This file is intentionally not included in the Git repository.
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
            })
            .UseStartup<Startup>()
                .Build();
    }
}
