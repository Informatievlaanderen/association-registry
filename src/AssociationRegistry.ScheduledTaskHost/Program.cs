namespace AssociationRegistry.ScheduledTaskHost;

using AssociationRegistry.Notifications;
using Configuration;
using Coravel;
using EventStore;
using Grar;
using Grar.AddressSync;
using Helpers;
using HostedServices;
using Infrastructure.Extensions;
using Invocables;
using Polly;
using Polly.Retry;
using Serilog;
using Serilog.Debugging;
using System.Net;
using System.Text;
using Vereniging;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
               .AddJsonFile("appsettings.json")
               .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName.ToLowerInvariant()}.json", optional: true,
                            reloadOnChange: false)
               .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
               .AddEnvironmentVariables()
               .AddCommandLine(args);

        SelfLog.Enable(Console.WriteLine);

        ConfigureEncoding();
        ConfigureAppDomainExceptions();

        builder.WebHost.ConfigureKestrel(
            options =>
                options.AddEndpoint(IPAddress.Any, port: 11009));

        ConfigureServices(builder.Services, builder.Configuration);
        ConfigureHostedServices(builder.Services);
        ConfigureScheduler(builder.Services, builder.Configuration);

        var app = builder.Build();

        ConfigureSchedulerInvocables(app.Services);

        await app.RunAsync();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var postgreSqlOptions = configuration.GetPostgreSqlOptions();
        var addressSynchronisationOptions = configuration.GetAddressSynchronisationOptions();
        var powerBiExportOptions = configuration.GetPowerBiExportOptions();

        services
           .AddSingleton(postgreSqlOptions)
           .AddSingleton(addressSynchronisationOptions)
           .AddSingleton(powerBiExportOptions);

        services
           .AddMarten(postgreSqlOptions);

        services
           .AddOpenTelemetryServices();

        services
           .AddSingleton<IEventStore, EventStore>()
           .AddTransient<INotifier, SlackNotifier>()
           .AddSingleton<ITeSynchroniserenLocatiesFetcher, TeSynchroniserenLocatiesFetcher>()
           .AddSingleton<IVerenigingsRepository, VerenigingsRepository>()
           .AddSingleton(new SlackWebhook(addressSynchronisationOptions.SlackWebhook))
           .AddSingleton<TeSynchroniserenLocatieAdresMessageHandler>()
           .AddSingleton<IGrarClient, GrarClient>()
           .AddSingleton<IGrarHttpClient>(provider => provider.GetRequiredService<GrarHttpClient>())
           .AddHttpClient<GrarHttpClient>()
           .ConfigureHttpClient(httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("x-api-key", addressSynchronisationOptions.ApiKey);
                httpClient.BaseAddress = new Uri(addressSynchronisationOptions.BaseUrl);
            });
    }

    private static void ConfigureHostedServices(IServiceCollection services)
    {
        services.AddResiliencePipeline(nameof(AddressMigrationKafkaConsumer), static builder =>
        {
            builder.AddRetry(new RetryStrategyOptions()
            {
                MaxRetryAttempts = int.MaxValue,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder().Handle<Exception>(exception =>
                {
                    Log.Error(exception, $"{nameof(AddressMigrationKafkaConsumer)} failed");

                    // await _notifier.Notify(new AdresKafkaConsumerGefaald(exception));
                    return true;
                }),
            });
        });

        services.AddHostedService<AddressMigrationKafkaConsumer>();
    }

    private static void ConfigureScheduler(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScheduler();
        services.AddMailer(configuration);
        services.AddQueue();

        services.AddTransient<AddressSynchronisationInvocable>();
        services.AddTransient<PowerBiExportInvocable>();
    }

    private static void ConfigureSchedulerInvocables(IServiceProvider services)
    {
        services
           .UseScheduler(scheduler =>
            {
                scheduler.OnWorker(nameof(AddressSynchronisationInvocable));

                scheduler.Schedule<AddressSynchronisationInvocable>()
                         .Cron(services.GetRequiredService<PowerBiExportOptions>().CronExpression)
                         .PreventOverlapping(nameof(AddressSynchronisationInvocable))
                         .Zoned(TimeZoneInfo.Local);

                scheduler.OnWorker(nameof(PowerBiExportInvocable));

                scheduler.Schedule<PowerBiExportInvocable>()
                         .Cron(services.GetRequiredService<PowerBiExportOptions>().CronExpression)
                         .PreventOverlapping(nameof(PowerBiExportInvocable))
                         .Zoned(TimeZoneInfo.Local);
            })
           .OnError(exception =>
            {
                Console.Error.WriteLine(exception);
            });
    }

    private static void ConfigureEncoding()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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
