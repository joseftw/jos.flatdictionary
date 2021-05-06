using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace JOS.FlatDictionary.Infrastructure.Logging
{
    public class LoggerConfigurator
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public LoggerConfigurator(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        public Task Configure()
        {
            var loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.MinimumLevel.Is(DefaultLevel);

            var overrides = GetOverrides();

            foreach (var @override in overrides)
            {
                loggerConfiguration.MinimumLevel.Override(@override.Path, @override.Level);
            }

            loggerConfiguration.Enrich.FromLogContext();
            loggerConfiguration.Enrich.With<UtcTimestampEnricher>();

            if (ShouldLogToConsole)
            {
                if (ShouldLogJson)
                {
                    loggerConfiguration.WriteTo.Console(new JsonFormatter());
                }
                else
                {
                    loggerConfiguration.WriteTo.Console(outputTemplate: "[{UtcTimestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}");
                }
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            return Task.CompletedTask;
        }

        private LogEventLevel DefaultLevel => _configuration.GetValue<LogEventLevel?>("Logging:Level:Default") ?? LogEventLevel.Debug;

        private IEnumerable<LoggingOverride> GetOverrides()
        {
            return _configuration.GetSection("Logging:Level:Overrides").Get<LoggingOverride[]>();
        }

        private bool ShouldLogToConsole => _configuration.GetValue<bool?>("Logging:Output:Console:Enabled") ?? true;
        private bool ShouldLogJson => _configuration.GetValue<bool?>("Logging:Output:Console:Json") ?? !_hostEnvironment.IsDevelopment();
    }
}
