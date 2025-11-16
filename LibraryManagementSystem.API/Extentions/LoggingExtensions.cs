using Serilog;

namespace LibraryManagementSystem.API.Extensions
{
    public static class LoggingExtensions
    {
        public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
        {
            // Register Serilog .
            builder.Host.UseSerilog((context,loggerConfiguration) =>
            {
                loggerConfiguration.WriteTo.Console();
                loggerConfiguration.WriteTo.File(
                    "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                );

                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            });

            return builder;
        }
    }
}
