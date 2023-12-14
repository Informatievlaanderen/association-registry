namespace AssociationRegistry.Public.ProjectionHost;

using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Infrastructure.Json;
using Infrastructure.Program;
using Infrastructure.Program.WebApplication;
using Infrastructure.Program.WebApplicationBuilder;
using JasperFx.CodeGeneration;
using Marten;
using Metrics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oakton;
using OpenTelemetry.Extensions;
using Projections;
using Projections.Detail;
using Projections.Search;
using Serilog;
using Serilog.Debugging;
using System.Net;
using System.Text;
using Wolverine;

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

        builder.Configuration.GetValidPostgreSqlOptionsOrThrow();
        var elasticSearchOptions = builder.Configuration.GetValidElasticSearchOptionsOrThrow();

        SelfLog.Enable(Console.WriteLine);

        ConfigureEncoding();
        ConfigureJsonSerializerSettings();
        ConfigureAppDomainExceptions();

        builder.WebHost.ConfigureKestrel(
            options =>
                options.AddEndpoint(IPAddress.Any, port: 11005));

        builder.Host.ApplyOaktonExtensions();

        builder.Host.UseWolverine(
            options =>
            {
                options.Discovery.IncludeType<PubliekZoekProjectionHandler>();

                options.OptimizeArtifactWorkflow(TypeLoadMode.Static);
            });

        builder.Services
               .ConfigureRequestLocalization()
               .AddOpenTelemetry(new PubliekInstrumentation())
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
            pattern: "v1/projections/detail/rebuild",
            handler: async (IDocumentStore store, ILogger<Program> logger, CancellationToken cancellationToken) =>
            {
                var projectionDaemon = await store.BuildProjectionDaemonAsync();
                await projectionDaemon.RebuildProjection<PubliekVerenigingDetailProjection>(cancellationToken);
                logger.LogInformation("Rebuild complete");
            });

        app.MapPost(
            pattern: "v1/projections/search/rebuild",
            handler: async (
                IDocumentStore store,
                IElasticClient elasticClient,
                ElasticSearchOptionsSection options,
                ILogger<Program> logger,
                CancellationToken cancellationToken) =>
            {
                var projectionDaemon = await store.BuildProjectionDaemonAsync();
                await projectionDaemon.StopShard($"{ProjectionNames.VerenigingZoeken}:All");

                await elasticClient.Indices.DeleteAsync(options.Indices.Verenigingen, ct: cancellationToken);
                await elasticClient.Indices.CreateVerenigingIndex(options.Indices.Verenigingen);

                await projectionDaemon.RebuildProjection(ProjectionNames.VerenigingZoeken, cancellationToken);
                logger.LogInformation("Rebuild complete");
            });

        app.MapGet(
            pattern: "v1/projections/status", handler: (IDocumentStore store, ILogger<Program> logger, CancellationToken cancellationToken)
                =>
                store.Advanced.AllProjectionProgress(token: cancellationToken));

        app.SetUpSwagger();
        await app.EnsureElasticSearchIsInitialized();
        ConfigureHealtChecks(app);

        await DistributedLock<Program>.RunAsync(
            runFunc: async () => await app.RunOaktonCommands(args),
            DistributedLockOptions.LoadFromConfiguration(builder.Configuration),
            app.Services.GetRequiredService<ILogger<Program>>());
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
                AppDomain.CurrentDomain.FriendlyName);

        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
            Log.Fatal(
                (Exception)eventArgs.ExceptionObject,
                messageTemplate: "Encountered a fatal exception, exiting program");
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
                            healthReport.Entries.Select(
                                pair =>
                                    new JProperty(
                                        pair.Key,
                                        new JObject(
                                            new JProperty(name: "status", pair.Value.Status.ToString()),
                                            new JProperty(name: "duration", pair.Value.Duration),
                                            new JProperty(name: "description", pair.Value.Description),
                                            new JProperty(name: "exception", pair.Value.Exception?.Message),
                                            new JProperty(
                                                name: "data",
                                                new JObject(
                                                    pair.Value.Data.Select(
                                                        p => new JProperty(p.Key, p.Value))))))))));

                return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
            },
        };

        app.UseHealthChecks(path: "/health", healthCheckOptions);
    }
}
