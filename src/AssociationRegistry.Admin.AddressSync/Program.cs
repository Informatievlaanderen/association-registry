namespace AssociationRegistry.Admin.AddressSync;

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
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Debugging;
using System.Diagnostics;
using Vereniging;

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

        await host.StartAsync();
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var postgreSqlOptions = context.Configuration.GetPostgreSqlOptions();

        services
           .AddOpenTelemetryServices()
           .AddMarten(postgreSqlOptions);


        var grarHttpOptions = context.Configuration.GetGrarHttpOptions();

        services
           .AddHttpClient<GrarHttpClient>()
           .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(grarHttpOptions.BaseUrl));

        services
           .AddSingleton(postgreSqlOptions)
           .AddSingleton<IClock>(SystemClock.Instance)
           .AddSingleton<IEventPostConflictResolutionStrategy[]>([new AddressMatchConflictResolutionStrategy()])
           .AddSingleton<IEventPreConflictResolutionStrategy[]>([new AddressMatchConflictResolutionStrategy()])
           .AddSingleton<EventConflictResolver>()
           .AddSingleton<IGrarHttpClient>(provider => provider.GetRequiredService<GrarHttpClient>())
           .AddSingleton<IGrarClient, GrarClient>()
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
}
