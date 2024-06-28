namespace AssociationRegistry.Admin.AddressSync;

using Destructurama;
using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Debugging;
using System.Diagnostics;

public static class Program
{
    public static async Task Main(string[] args)
    {
        SelfLog.Enable(Console.WriteLine);


        var host = Host.CreateDefaultBuilder()
                       .ConfigureAppConfiguration(builder => builder
                                                            .AddJsonFile("appsettings.json")
                                                            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json",
                                                                         optional: true, reloadOnChange: false)
                                                            .AddEnvironmentVariables())
                       .ConfigureServices(ConfigureServices)
                       .ConfigureLogging(ConfigureLogger)
                       .Build();

        var sw = Stopwatch.StartNew();
        await host.StartAsync();
        sw.Stop();

        var logger = host.Services.GetRequiredService<ILogger<AddressSyncService>>();
        logger.LogInformation($"Het archiveren van uitnodigingen werd voltooid in {sw.ElapsedMilliseconds} ms.");
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var postgreSqlOptions = context.Configuration.GetPostgreSqlOptions();
        var addressSyncOptions = context.Configuration.GetAddressSyncOptions();

        services
           .AddOpenTelemetryServices()
           .AddMarten(postgreSqlOptions);
        services
           .AddSingleton(addressSyncOptions)
           .AddSingleton(postgreSqlOptions)
           .AddSingleton<IClock>(SystemClock.Instance)
           .AddHostedService<AddressSyncService>();
    }

    private static void ConfigureLogger(HostBuilderContext context, ILoggingBuilder builder)
    {
        var loggerConfig =
            new LoggerConfiguration()
               .ReadFrom.Configuration(context.Configuration)
               .Enrich.FromLogContext()
               .Enrich.WithMachineName()
               .Enrich.WithThreadId()
               .Enrich.WithEnvironmentUserName()
               .Destructure.JsonNetTypes();

        var logger = loggerConfig.CreateLogger();

        Log.Logger = logger;

        builder.AddOpenTelemetry(options =>
        {
            var resourceBuilder = ResourceBuilder.CreateDefault();
            ServiceCollectionExtensions.ConfigureResource()(resourceBuilder);
            options.SetResourceBuilder(resourceBuilder);
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;

            options.AddOtlpExporter((exporterOptions, _)  =>
            {
                exporterOptions.Protocol = OtlpExportProtocol.Grpc;
                exporterOptions.Endpoint = new Uri(ServiceCollectionExtensions.CollectorUrl);
            });
        });
    }
}
