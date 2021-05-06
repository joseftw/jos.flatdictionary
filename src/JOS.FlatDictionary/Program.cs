using System.Threading.Tasks;
using JOS.FlatDictionary.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace JOS.FlatDictionary
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddCommandLine(args).Build();
            var environment = GetEnvironment(configuration);
            
            var builder = new HostBuilder()
                .UseEnvironment(environment)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddConfiguration(configuration);
                    config.AddJsonFile("appsettings.json");
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment}.json", optional: true);
                    config.AddJsonFile("appsettings.Local.json", optional: true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    _ = new LoggerConfigurator(
                        hostingContext.Configuration,
                        hostingContext.HostingEnvironment).Configure();
                    logging.AddSerilog(dispose: true);
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateOnBuild = true;
                    options.ValidateScopes = true;
                });

            var host = builder.Build();
            await host.RunAsync();
        }

        private static string GetEnvironment(IConfiguration configuration)
        {
            var environment = configuration.GetValue<string>("environment");

            if (string.IsNullOrWhiteSpace(environment))
            {
                environment = Environments.Development;
            }

            return environment;
        }
    }
}
