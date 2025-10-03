using Serilog;

namespace LibraryManagementSystem.API.Extensions
{
    public static class LoggingExtensions
    {
        public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
        {
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            // 2. Clear the default Microsoft logging providers.
            builder.Logging.ClearProviders();

            // 3. Register Serilog as the logging provider for your application.
            builder.Host.UseSerilog(logger);

            return builder;
        }
    }
}
