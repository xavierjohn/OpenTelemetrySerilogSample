using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Reflection;
using OpenTelemetry.Logs;

namespace OpenTelemetrySerilogSample
{
    public class Program
    {
        public static int Main(string[] args)
        {
            SetupLogging();

            try
            {
                Log.Information("**** Starting Open Telemetry Serilog Sample ****");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                }, writeToProviders: true)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.ClearProviders();
                    builder.AddOpenTelemetry(options =>
                    {
                        options.AddConsoleExporter();
                    });
                });

        private static void SetupLogging()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var loggerConfig = new LoggerConfiguration()
                                .MinimumLevel.Information()
                                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                                .Enrich.FromLogContext()
                                .Enrich.WithProperty("Version", assembly.Version);

            Log.Logger = loggerConfig.CreateLogger();
        }
    }
}
