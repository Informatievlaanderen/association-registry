namespace AssociationRegistry.Admin.AddressSync;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Geotags;
using Destructurama;
using EventStore;
using EventStore.ConflictResolution;
using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Resources;
using Integrations.Grar;
using Integrations.Grar.Clients;
using Hosts;
using Infrastructure.Extensions;
using Integrations.Slack;
using JasperFx;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using MessageHandling.Sqs.AddressSync;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using NodaTime;
using Persoonsgegevens;
using Serilog;
using Serilog.Debugging;
using Vereniging;
using Wolverine;

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
                .ApplyJasperFxExtensions()
                .Build();

        ConfigureAppDomainExceptions();

        await host.RunJasperFxCommands(args);
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var postgreSqlOptions = context.Configuration.GetPostgreSqlOptions();

        services
           .AddOpenTelemetryServices()
           .AddMarten(postgreSqlOptions)
           .AddWolverine(postgreSqlOptions);

        var addressSyncOptions = context.Configuration.GetAddressSyncOptions();

        services
           .AddHttpClient<GrarHttpClient>()
           .ConfigureHttpClient(httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("x-api-key", addressSyncOptions.ApiKey);
                httpClient.BaseAddress = new Uri(addressSyncOptions.BaseUrl);
            });

        services
           .AddSingleton(postgreSqlOptions)
           .AddSingleton<IClock>(SystemClock.Instance)
           .AddSingleton<IEventPostConflictResolutionStrategy[]>([new AddressMatchConflictResolutionStrategy()])
           .AddSingleton<IEventPreConflictResolutionStrategy[]>([new AddressMatchConflictResolutionStrategy()])
           .AddSingleton<EventConflictResolver>()
           .AddSingleton<IGrarHttpClient>(provider => provider.GetRequiredService<GrarHttpClient>())
           .AddSingleton(new GrarOptions().GrarClient)
           .AddSingleton(new SlackWebhook(addressSyncOptions.SlackWebhook))
           .AddSingleton<IGrarClient, GrarClient>()
           .AddTransient<IEventStore, EventStore>()
           .AddScoped<IVerenigingsRepository, VerenigingsRepository>()
           .AddScoped<IVertegenwoordigerPersoonsgegevensRepository, VertegenwoordigerPersoonsgegevensRepository>()
           .AddScoped<IVertegenwoordigerPersoonsgegevensQuery, VertegenwoordigerPersoonsgegevensQuery>()
           .AddScoped<IPersoonsgegevensProcessor, PersoonsgegevensProcessor>()
           .AddScoped<PersoonsgegevensEventTransformers>()
           .AddScoped<TeSynchroniserenLocatieAdresMessageHandler>()
           .AddScoped<ITeSynchroniserenLocatiesFetcher, TeSynchroniserenLocatiesFetcher>()
           .AddTransient<INotifier, SlackNotifier>()
           .AddTransient<IGeotagsService, GeotagsService>()
           .AddTransient<IVerenigingsRepository, VerenigingsRepository>()
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
