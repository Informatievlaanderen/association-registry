namespace AssociationRegistry.Scheduled.Host;

using System.Net;
using System.Text;
using Admin.Schema.PowerBiExport;
using Amazon.S3;
using Bewaartermijnen;
using CommandHandling.Bewaartermijnen.Acties.Verlopen;
using EventStore.ConflictResolution;
using Infrastructure.Extensions;
using Infrastructure.MartenSetup;
using Infrastructure.Program.WebApplicationBuilder;
using Integrations.Slack;
using JasperFx;
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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using Persoonsgegevens;
using PowerBi;
using PowerBi.Writers;
using Quartz;
using Queries;
using Serilog;
using Serilog.Debugging;
using WebApi.Schedulers;
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
        ConfigureServices(builder);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, 11012);
        });

        builder.Host.ApplyJasperFxExtensions();

        builder.Services.ConfigureRequestLocalization().AddMvc().AddDataAnnotationsLocalization();

        builder.Services.AddHealthChecks();

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
        var powerBiExportOptions = builder.Configuration.GetPowerBiExportOptions();

        services.AddOpenTelemetryServices().AddMarten(postgreSqlOptions).AddWolverine(postgreSqlOptions);

        services
            .AddQuartz(q =>
            {
                var bewaartermijnPurgeRunner = new JobKey(ExpiredBewaartermijnJob.JobName);
                q.AddJob<ExpiredBewaartermijnJob>(opts => opts.WithIdentity(bewaartermijnPurgeRunner));
                q.AddTrigger(opts => opts.ForJob(bewaartermijnPurgeRunner).WithCronSchedule(bewaartermijnOptions.Cron));
            })
            .AddQuartz(q =>
            {
                var powerBiExportJob = new JobKey(PowerBiExportJob.JobName);
                q.AddJob<PowerBiExportJob>(opts => opts.WithIdentity(powerBiExportJob));
                q.AddTrigger(opts => opts.ForJob(powerBiExportJob).WithCronSchedule(powerBiExportOptions.Cron));
            });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        services
            .AddSingleton(postgreSqlOptions)
            .AddSingleton<IClock>(SystemClock.Instance)
            .AddSingleton<IAmazonS3, AmazonS3Client>()
            .AddSingleton(sp => CreatePowerBiExporters(sp, powerBiExportOptions))
            .AddSingleton(sp => CreatePowerBiDubbelDetectieExporters(sp, powerBiExportOptions))
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

    private static PowerBiExporters CreatePowerBiExporters(
        IServiceProvider sp,
        PowerBiExportOptions powerBiExportOptions
    ) =>
        new(
            new List<Exporter<PowerBiExportDocument>>
            {
                CreateExporter<BasisgegevensRecordWriter>(sp, powerBiExportOptions, WellKnownFileNames.Basisgegevens),
                CreateExporter<ContactgegevensRecordWriter>(
                    sp,
                    powerBiExportOptions,
                    WellKnownFileNames.Contactgegevens
                ),
                CreateExporter<HoofdactiviteitenRecordWriter>(
                    sp,
                    powerBiExportOptions,
                    WellKnownFileNames.Hoofdactiviteiten
                ),
                CreateExporter<WerkingsgebiedenRecordWriter>(
                    sp,
                    powerBiExportOptions,
                    WellKnownFileNames.Werkingsgebieden
                ),
                CreateExporter<LocatiesRecordWriter>(sp, powerBiExportOptions, WellKnownFileNames.Locaties),
                CreateExporter<HistoriekRecordWriter>(sp, powerBiExportOptions, WellKnownFileNames.Historiek),
                CreateExporter<LidmaatschappenRecordWriter>(
                    sp,
                    powerBiExportOptions,
                    WellKnownFileNames.Lidmaatschappen
                ),
                CreateExporter<BankrekeningnummerRecordWriter>(
                    sp,
                    powerBiExportOptions,
                    WellKnownFileNames.Bankrekeningnummers
                ),
                CreateExporter<ErkenningenRecordWriter>(sp, powerBiExportOptions, WellKnownFileNames.Erkenningen),
            }
        );

    private static Exporter<PowerBiExportDocument> CreateExporter<TWriter>(
        IServiceProvider sp,
        PowerBiExportOptions options,
        string fileName
    )
        where TWriter : class, IRecordWriter<PowerBiExportDocument>, new() =>
        new(
            fileName,
            options.BucketName,
            new TWriter(),
            sp.GetRequiredService<IAmazonS3>(),
            sp.GetRequiredService<ILogger<Exporter<PowerBiExportDocument>>>()
        );

    private static PowerBiDubbelDetectieExporters CreatePowerBiDubbelDetectieExporters(
        IServiceProvider sp,
        PowerBiExportOptions options
    ) =>
        new(
            new List<Exporter<PowerBiExportDubbelDetectieDocument>>
            {
                CreateDubbelDetectieExporter<DubbelDetectieRecordWriter>(
                    sp,
                    options,
                    WellKnownFileNames.DubbelDetectie
                ),
            }
        );

    private static Exporter<PowerBiExportDubbelDetectieDocument> CreateDubbelDetectieExporter<TWriter>(
        IServiceProvider sp,
        PowerBiExportOptions options,
        string fileName
    )
        where TWriter : class, IRecordWriter<PowerBiExportDubbelDetectieDocument>, new() =>
        new(
            fileName,
            options.BucketName,
            new TWriter(),
            sp.GetRequiredService<IAmazonS3>(),
            sp.GetRequiredService<ILogger<Exporter<PowerBiExportDubbelDetectieDocument>>>()
        );

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
