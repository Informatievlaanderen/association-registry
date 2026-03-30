namespace AssociationRegistry.Bewaartermijnen.PurgeRunner;

using CommandHandling.Bewaartermijnen.Acties.Verlopen;
using Destructurama;
using EventStore.ConflictResolution;
using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Resources;
using Infrastructure.Extensions;
using Infrastructure.MartenSetup;
using Integrations.Slack;
using JasperFx;
using MartenDb.BankrekeningnummerPersoonsgegevens;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using Persoonsgegevens;
using Queries;
using Serilog;
using Serilog.Debugging;

public static class Program
{
    public static async Task Main(string[] args)
    {
        SelfLog.Enable(Console.WriteLine);

        var host = BuildHost();

        ConfigureAppDomainExceptions();

        await host.RunJasperFxCommands(args);
    }

    public static IHost BuildHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(
                (context, builder) =>
                    builder
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile(
                            $"appsettings.{context.HostingEnvironment.EnvironmentName.ToLowerInvariant()}.json",
                            optional: true,
                            reloadOnChange: false
                        )
                        .AddJsonFile(
                            $"appsettings.{Environment.MachineName.ToLowerInvariant()}.json",
                            optional: true,
                            reloadOnChange: false
                        )
                        .AddEnvironmentVariables()
            )
            .ConfigureServices(ConfigureServices)
            .ConfigureLogging(ConfigureLogger)
            .ApplyJasperFxExtensions()
            .Build();
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var postgreSqlOptions = context.Configuration.GetPostgreSqlOptions();
        var bewaartermijnOptions = context.Configuration.GetBewaartermijnenOptions();

        var autoCreate = context.Configuration.GetValue<bool>("IsTesting") ? AutoCreate.All : AutoCreate.None;

        services.AddOpenTelemetryServices().AddMarten(postgreSqlOptions, autoCreate).AddWolverine(postgreSqlOptions);

        services
            .AddSingleton(postgreSqlOptions)
            .AddSingleton<IClock>(SystemClock.Instance)
            .AddSingleton<IEventPostConflictResolutionStrategy[]>([new AddressMatchConflictResolutionStrategy()])
            .AddSingleton<IEventPreConflictResolutionStrategy[]>([new AddressMatchConflictResolutionStrategy()])
            .AddSingleton<EventConflictResolver>()
            .AddSingleton(new SlackWebhook(bewaartermijnOptions.SlackWebhook))
            .AddScoped<IEventStore, EventStore>()
            .AddScoped<IAggregateSession, AggregateSession>()
            .AddScoped<IVertegenwoordigerPersoonsgegevensRepository, VertegenwoordigerPersoonsgegevensRepository>()
            .AddScoped<IVertegenwoordigerPersoonsgegevensQuery, VertegenwoordigerPersoonsgegevensQuery>()
            .AddScoped<IBankrekeningnummerPersoonsgegevensRepository, BankrekeningnummerPersoonsgegevensRepository>()
            .AddScoped<IBankrekeningnummerPersoonsgegevensQuery, BankrekeningnummerPersoonsgegevensQuery>()
            .AddScoped<PersoonsgegevensEventTransformers>()
            .AddScoped<IPersoonsgegevensProcessor, PersoonsgegevensProcessor>()
            .AddScoped<VerloopBewaartermijnCommandHandler>()
            .AddScoped<IVerlopenBewaartermijnenProcessor, VerlopenBewaartermijnenProcessor>()
            .AddScoped<IVerlopenBewaartermijnQuery, VerlopenBewaartermijnQuery>()
            .AddScoped<INotifier, SlackNotifier>()
            .AddHostedService<ExpiredBewaartermijnBackgroundService>();
    }

    private static void ConfigureLogger(HostBuilderContext context, ILoggingBuilder builder)
    {
        var loggerConfig = new LoggerConfiguration()
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

            options.AddOtlpExporter(
                (exporterOptions, _) =>
                {
                    exporterOptions.Protocol = OtlpExportProtocol.Grpc;
                    exporterOptions.Endpoint = new Uri(ServiceCollectionExtensions.CollectorUrl);
                }
            );
        });
    }

    private static void ConfigureAppDomainExceptions()
    {
        AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
            Log.Debug(
                exception: eventArgs.Exception,
                messageTemplate: "FirstChanceException event raised in {AppDomain}",
                propertyValue: AppDomain.CurrentDomain.FriendlyName
            );

        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
            Log.Fatal(
                (Exception)eventArgs.ExceptionObject,
                messageTemplate: "Encountered a fatal exception, exiting program"
            );
    }
}
