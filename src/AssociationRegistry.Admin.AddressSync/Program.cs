﻿namespace AssociationRegistry.Admin.AddressSync;

using Api.Infrastructure.Extensions;
using Destructurama;
using EventStore;
using Grar;
using Grar.AddressSync;
using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using Notifications;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Debugging;
using Vereniging;

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

        await host.StartAsync();
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var postgreSqlOptions = context.Configuration.GetPostgreSqlOptions();

        services
           .AddOpenTelemetryServices()
           .AddMarten(postgreSqlOptions);

        // TODO:
        var grarOptions = context.Configuration.GetGrarHttpOptions();

        services
           .AddHttpClient<GrarHttpClient>()
           .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(grarOptions.HttpClient.BaseUrl));

        services
           .AddSingleton(postgreSqlOptions)
           .AddSingleton<IClock>(SystemClock.Instance)
           .AddSingleton<IEventPostConflictResolutionStrategy[]>([new AddressMatchConflictResolutionStrategy()])
           .AddSingleton<IEventPreConflictResolutionStrategy[]>([new AddressMatchConflictResolutionStrategy()])
           .AddSingleton<EventConflictResolver>()
           .AddSingleton<IGrarHttpClient>(provider => provider.GetRequiredService<GrarHttpClient>())
           .AddSingleton(new SlackWebhook(grarOptions.Kafka.SlackWebhook))
           .AddSingleton<IGrarClient, GrarClient>()
           .AddTransient<INotifier, SlackNotifier>()
           .AddSingleton<ITeSynchroniserenLocatiesFetcher, TeSynchroniserenLocatiesFetcher>()
           .AddSingleton<IEventStore, EventStore>()
           .AddSingleton<IVerenigingsRepository, VerenigingsRepository>()
           .AddSingleton<TeSynchroniserenLocatieAdresMessageHandler>()
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

            options.AddOtlpExporter((exporterOptions, _) =>
            {
                exporterOptions.Protocol = OtlpExportProtocol.Grpc;
                exporterOptions.Endpoint = new Uri(ServiceCollectionExtensions.CollectorUrl);
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
