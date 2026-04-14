namespace AssociationRegistry.Scheduled.Host;

using System.Net;
using System.Text;
using Admin.ProjectionHost.Projections.Search.Zoeken;
using Asp.Versioning.ApplicationModels;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Bewaartermijnen;
using CommandHandling.Bewaartermijnen.Acties.Verlopen;
using EventStore.ConflictResolution;
using Hosts;
using Infrastructure.Extensions;
using Infrastructure.MartenSetup;
using Infrastructure.Program.WebApplicationBuilder;
using Integrations.Slack;
using JasperFx;
using JasperFx.CodeGeneration;
using MartenDb.BankrekeningnummerPersoonsgegevens;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using Persoonsgegevens;
using Quartz;
using Queries;
using Serilog;
using Serilog.Debugging;
using WebApi.Schedulers;
using Wolverine;
using HealthStatus = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CreateBuilder(args);
        var app = BuildApp(builder);

        await app.RunJasperFxCommands(args);
    }

    private static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder
            .Configuration.AddJsonFile("appsettings.json")
            .AddJsonFile(
                $"appsettings.{builder.Environment.EnvironmentName.ToLowerInvariant()}.json",
                optional: true,
                reloadOnChange: false
            )
            .AddJsonFile(
                $"appsettings.{Environment.MachineName.ToLowerInvariant()}.json",
                optional: true,
                reloadOnChange: false
            )
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        SelfLog.Enable(Console.WriteLine);

        ConfigureEncoding();
        ConfigureJsonSerializerSettings();
        ConfigureAppDomainExceptions();

        ConfigureServices(builder);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, 11012);
        });

        builder.Host.ApplyJasperFxExtensions();

        // TODO: builder.ConfigureOpenTelemetry(new AdminInstrumentation());

        builder.Services.ConfigureRequestLocalization().AddMvc().AddDataAnnotationsLocalization();

        builder.Services.AddHealthChecks();

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Transient<IApiControllerSpecification, ApiControllerSpec>()
        );

        builder.Host.UseConsoleLifetime();

        return builder;
    }

    private static WebApplication BuildApp(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        app.AddScheduleEndpoints();

        ConfigureHealtChecks(app);
        ConfigureLifetimeHooks(app);

        return app;
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        var postgreSqlOptions = builder.Configuration.GetPostgreSqlOptions();
        var bewaartermijnOptions = builder.Configuration.GetBewaartermijnenOptions();

        services.AddOpenTelemetryServices().AddMarten(postgreSqlOptions).AddWolverine(postgreSqlOptions);

        services.AddQuartz(q =>
        {
            var bewaartermijnPurgeRunner = new JobKey(ExpiredBewaartermijnJob.JobName);
            q.AddJob<ExpiredBewaartermijnJob>(opts => opts.WithIdentity(bewaartermijnPurgeRunner));

            q.AddTrigger(opts => opts.ForJob(bewaartermijnPurgeRunner).WithCronSchedule(bewaartermijnOptions.Cron));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

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
            .AddScoped<INotifier, SlackNotifier>();
    }

    private static void ConfigureEncoding()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    private static void ConfigureJsonSerializerSettings()
    {
        var jsonSerializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();
        JsonConvert.DefaultSettings = () => jsonSerializerSettings;
    }

    private static void ConfigureAppDomainExceptions()
    {
        AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
            Log.Debug(
                eventArgs.Exception,
                messageTemplate: "FirstChanceException event raised in {AppDomain}",
                AppDomain.CurrentDomain.FriendlyName
            );

        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
            Log.Fatal(
                (Exception)eventArgs.ExceptionObject,
                messageTemplate: "Encountered a fatal exception, exiting program"
            );
    }

    private static void ConfigureHealtChecks(WebApplication app)
    {
        var healthCheckOptions = new HealthCheckOptions
        {
            AllowCachingResponses = false,

            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
            },

            ResponseWriter = (httpContext, healthReport) =>
            {
                httpContext.Response.ContentType = "application/json";

                var json = new JObject(
                    new JProperty(name: "status", healthReport.Status.ToString()),
                    new JProperty(name: "totalDuration", healthReport.TotalDuration.ToString()),
                    new JProperty(
                        name: "results",
                        new JObject(
                            healthReport.Entries.Select(pair => new JProperty(
                                pair.Key,
                                new JObject(
                                    new JProperty(name: "status", pair.Value.Status.ToString()),
                                    new JProperty(name: "duration", pair.Value.Duration),
                                    new JProperty(name: "description", pair.Value.Description),
                                    new JProperty(name: "exception", pair.Value.Exception?.Message),
                                    new JProperty(
                                        name: "data",
                                        new JObject(pair.Value.Data.Select(p => new JProperty(p.Key, p.Value)))
                                    )
                                )
                            ))
                        )
                    )
                );

                return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
            },
        };

        app.UseHealthChecks(path: "/health", healthCheckOptions);
    }

    private static void ConfigureLifetimeHooks(WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() => Log.Information("Scheduled Host Application started"));

        app.Lifetime.ApplicationStopping.Register(() =>
        {
            Log.Information("Scheduled Host Application stopping");
            Log.CloseAndFlush();
        });

        Console.CancelKeyPress += (_, eventArgs) =>
        {
            app.Lifetime.StopApplication();

            // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
            eventArgs.Cancel = true;
        };
    }
}
