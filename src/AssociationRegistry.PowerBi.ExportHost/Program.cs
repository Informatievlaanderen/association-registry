﻿namespace AssociationRegistry.PowerBi.ExportHost;

using Amazon.S3;
using EventStore;
using Grar;
using Grar.AddressSync;
using Notifications;
using Vereniging;
using Destructurama;
using HostedServices;
using Hosts.Configuration;
using Infrastructure.Extensions;
using Marten;
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

public static class Program
{
    public static async Task Main(string[] args)
    {
        SelfLog.Enable(Console.WriteLine);

        var host =
            Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(
                     (context, builder) =>
                         builder
                            .AddJsonFile("appsettings.json")
                            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName.ToLowerInvariant()}.json",
                                         optional: true,
                                         reloadOnChange: false)
                            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json",
                                         optional: true, reloadOnChange: false)
                            .AddEnvironmentVariables())
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(ConfigureLogger)
                .Build();

        ConfigureAppDomainExceptions();

        await host.RunAsync();
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var postgreSqlOptions = context.Configuration.GetPostgreSqlOptionsSection();
        var powerBiExportOptions = context.Configuration.GetPowerBiExportOptions();

        services
           .AddOpenTelemetryServices()
           .AddSingleton<IDocumentStore>(new DocumentStore(ServiceCollectionMartenExtensions.GetStoreOptions(postgreSqlOptions)));

        services
           .AddSingleton(postgreSqlOptions)
           .AddSingleton(powerBiExportOptions)
           .AddSingleton<IClock>(SystemClock.Instance)
           .AddSingleton<IAmazonS3, AmazonS3Client>();

        services.AddHostedService<PowerBiExportService>();
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
            ServiceCollectionOpenTelemetryExtensions.ConfigureResource()(resourceBuilder);
            options.SetResourceBuilder(resourceBuilder);
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;

            options.AddOtlpExporter((exporterOptions, _) =>
            {
                exporterOptions.Protocol = OtlpExportProtocol.Grpc;
                exporterOptions.Endpoint = new Uri(ServiceCollectionOpenTelemetryExtensions.CollectorUrl);
            });
        });
    }

    private static void ConfigureAppDomainExceptions()
    {
        AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
            Log.Debug(
                eventArgs.Exception,
                messageTemplate: "FirstChanceException event raised in {AppDomain}",
                AppDomain.CurrentDomain.FriendlyName);

        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
            Log.Fatal(
                (Exception)eventArgs.ExceptionObject,
                messageTemplate: "Encountered a fatal exception, exiting program");
    }
}
