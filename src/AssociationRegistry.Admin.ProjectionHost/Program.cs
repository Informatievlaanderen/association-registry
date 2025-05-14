namespace AssociationRegistry.Admin.ProjectionHost;

using Asp.Versioning.ApplicationModels;
using Extensions;
using Hosts;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Infrastructure.Json;
using Infrastructure.Metrics;
using Infrastructure.Program;
using Infrastructure.Program.WebApplicationBuilder;
using JasperFx.CodeGeneration;
using Marten.Events.Daemon;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oakton;
using OpenTelemetry.Extensions;
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
                options.AddEndpoint(IPAddress.Any, port: 11006));

        builder.Host.ApplyOaktonExtensions();

        builder.Host.UseWolverine(
            opts =>
            {
                opts.ApplicationAssembly = typeof(Program).Assembly;
                opts.OptimizeArtifactWorkflow(TypeLoadMode.Static);
            });

        builder.ConfigureOpenTelemetry(new AdminInstrumentation());

        builder.Services
               .ConfigureRequestLocalization()
               .ConfigureProjectionsWithMarten(builder.Configuration, builder.Environment.IsDevelopment())
               .ConfigureSwagger()
               .ConfigureElasticSearch(elasticSearchOptions)
               .AddMvc()
               .AddDataAnnotationsLocalization();

        builder.Services
               .AddHealthChecks()
               .AddMartenAsyncDaemonHealthCheck();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IApiControllerSpecification, ApiControllerSpec>());

        builder.Host.UseConsoleLifetime();

        var app = builder.Build();

        if (ProgramArguments.IsCodeGen(args))
        {
            await app.RunOaktonCommands(args);
            return;
        }

        ElasticSearchExtensions.EnsureIndexExists(app.Services.GetRequiredService<IElasticClient>(),
                                                  elasticSearchOptions.Indices!.Verenigingen!,
                                                  elasticSearchOptions.Indices!.DuplicateDetection!);

        app.AddProjectionEndpoints(
            app.Configuration.GetSection(RebuildConfigurationSection.SectionName).Get<RebuildConfigurationSection>()!);

        // app.SetUpSwagger();
        ConfigureHealtChecks(app);
        ConfigureLifetimeHooks(app);

        await app.RunOaktonCommands(args);
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

    private static void ConfigureLifetimeHooks(WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() => Log.Information("Admin Projections Application started"));

        app.Lifetime.ApplicationStopping.Register(
            () =>
            {
                Log.Information("Admin Projections Application stopping");
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

public static class ResponseExtensions
{
    public static async Task<TResponse> ThrowIfInvalidAsync<TResponse>(this Task<TResponse> response)
        where TResponse : AcknowledgedResponseBase
    {
        var acknowledgedResponseBase = await response;

        return acknowledgedResponseBase.IsValid ? acknowledgedResponseBase : throw acknowledgedResponseBase.OriginalException;
    }
}
