using Serilog;
using Serilog.Events;

namespace Customer.Service.Ignition
{
    public static class LogIgnition
    {
        public static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            // Logging
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console()
                .WriteTo.File("logs.txt")
                .CreateLogger();

            builder.Host.UseSerilog();
        }
    }
}
