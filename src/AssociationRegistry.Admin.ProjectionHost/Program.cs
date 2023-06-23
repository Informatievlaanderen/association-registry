using System.Net;
using System.Text;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Json;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplication;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;
using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
using Marten;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Serilog;
using Serilog.Debugging;
using Wolverine;
using ApiControllerSpec = AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.ApiControllerSpec;


using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Linq;
using Oakton;

namespace AssociationRegistry.Admin.ProjectionHost;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Extensions;
using Projections.Search;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        builder.Configuration.GetValidPostgreSqlOptionsOrThrow();
        var elasticSearchOptions = builder.Configuration.GetValidElasticSearchOptionsOrThrow();

        SelfLog.Enable(Console.WriteLine);

        ConfigureEncoding();
        ConfigureJsonSerializerSettings();
        ConfigureAppDomainExceptions();

        builder.WebHost.ConfigureKestrel(
            options =>
                options.AddEndpoint(IPAddress.Any, 11006));

        builder.Host.ApplyOaktonExtensions();
        builder.Host.UseWolverine(
            opts =>
            {
                opts.ApplicationAssembly = typeof(Program).Assembly;
            });

        builder.Services
            .AddSingleton<IVerenigingBrolFeeder, VerenigingBrolFeeder>()
            .ConfigureRequestLocalization()
            .AddOpenTelemetry()
            .ConfigureProjectionsWithMarten(builder.Configuration)
            .ConfigureSwagger()
            .ConfigureElasticSearch(elasticSearchOptions)
            .AddMvc()
            .AddDataAnnotationsLocalization();

        builder.Services.AddHealthChecks();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IApiControllerSpecification, ApiControllerSpec>());

        builder.Host.UseConsoleLifetime();

        var app = builder.Build();

        app.MapPost(
            "/rebuild",
            async (IDocumentStore store, ILogger<Program> logger, CancellationToken cancellationToken) =>
            {
                var projectionDaemon = await store.BuildProjectionDaemonAsync();
                await projectionDaemon.RebuildProjection<BeheerVerenigingDetailProjection>(cancellationToken);
                logger.LogInformation("Rebuild complete");
            });

        app.SetUpSwagger();
        ConfigureHealtChecks(app);

        await DistributedLock<Program>.RunAsync(
            async () => await app.RunOaktonCommands(args),
            DistributedLockOptions.LoadFromConfiguration(builder.Configuration),
            app.Services.GetRequiredService<ILogger<Program>>());
    }

    static void ConfigureEncoding()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    static void ConfigureJsonSerializerSettings()
    {
        var jsonSerializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();
        JsonConvert.DefaultSettings = () => jsonSerializerSettings;
    }

    static void ConfigureAppDomainExceptions()
    {
        AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
            Log.Debug(
                eventArgs.Exception,
                "FirstChanceException event raised in {AppDomain}",
                AppDomain.CurrentDomain.FriendlyName);

        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
            Log.Fatal(
                (Exception)eventArgs.ExceptionObject,
                "Encountered a fatal exception, exiting program");
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
                    new JProperty("status", healthReport.Status.ToString()),
                    new JProperty("totalDuration", healthReport.TotalDuration.ToString()),
                    new JProperty(
                        "results",
                        new JObject(
                            healthReport.Entries.Select(
                                pair =>
                                    new JProperty(
                                        pair.Key,
                                        new JObject(
                                            new JProperty("status", pair.Value.Status.ToString()),
                                            new JProperty("duration", pair.Value.Duration),
                                            new JProperty("description", pair.Value.Description),
                                            new JProperty("exception", pair.Value.Exception?.Message),
                                            new JProperty(
                                                "data",
                                                new JObject(
                                                    pair.Value.Data.Select(
                                                        p => new JProperty(p.Key, p.Value))))))))));

                return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
            },
        };

        app.UseHealthChecks("/health", healthCheckOptions);
    }
}
