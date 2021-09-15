using System.Threading.Tasks;
using Helix.Bot.Extensions.Helix.Bot.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Helix.Bot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CreateLogger();

            var host = CreateHostBuilder(args)
                .UseSerilog()
                .Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args);

            hostBuilder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddUserSecrets<Program>();
            });

            hostBuilder.UseStartup<Startup>();
            return hostBuilder;
        }

        private static void CreateLogger()
        {
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .WriteTo.File("Logs/log.txt",
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day).CreateLogger();
        }
    }
}
