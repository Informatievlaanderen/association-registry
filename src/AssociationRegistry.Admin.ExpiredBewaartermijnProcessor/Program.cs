namespace AssociationRegistry.Admin.ExpiredBewaartermijnProcessor;

using Destructurama;
using EventStore.ConflictResolution;
using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Resources;
using Infrastructure.Extensions;
using Infrastructure.MartenSetup;
using Integrations.Grar.Clients;
using Integrations.Slack;
using JasperFx;
using MartenDb.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using Queries;
using Serilog;
using Serilog.Debugging;

public static class Program
{
    public static async Task Main(string[] args)
    {
        SelfLog.Enable(Console.WriteLine);

        var host = Host.CreateDefaultBuilder()
                       .ConfigureAppConfiguration((context, builder) =>
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

        ConfigureAppDomainExceptions();

        await host.RunJasperFxCommands(args);
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var postgreSqlOptions = context.Configuration.GetPostgreSqlOptions();
        var bewaartermijnOptions = context.Configuration.GetBewaartermijnenOptions();

        services.AddOpenTelemetryServices().AddMarten(postgreSqlOptions).AddWolverine(postgreSqlOptions);
        services
           .AddSingleton(postgreSqlOptions)
           .AddSingleton<IClock>(SystemClock.Instance)
           .AddSingleton(new SlackWebhook(bewaartermijnOptions.SlackWebhook))
           .AddSingleton<EventConflictResolver>()
           .AddTransient<IEventStore, EventStore>()
           .AddScoped<IAggregateSession, AggregateSession>()
           .AddScoped<IVerlopenBewaartermijnenProcessor, VerlopenBewaartermijnenProcessor>()
           .AddScoped<IVerlopenBewaartermijnQuery, VerlopenBewaartermijnQuery>()
           .AddTransient<INotifier, SlackNotifier>()
           .AddTransient<IAggregateSession, AggregateSession>()
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

            options.AddOtlpExporter((exporterOptions, _) =>
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
