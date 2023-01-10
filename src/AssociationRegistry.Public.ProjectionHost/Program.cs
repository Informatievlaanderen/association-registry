using System.Net;
using System.Text;
using AssociationRegistry.OpenTelemetry.Extensions;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Json;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplication;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
using Marten;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Serilog;
using Serilog.Debugging;
using Wolverine;
using ApiControllerSpec = AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.ApiControllerSpec;

namespace AssociationRegistry.Public.ProjectionHost;

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
                options.AddEndpoint(IPAddress.Any, 11005));

        builder.Host.UseWolverine();
        builder.Services
            .AddTransient<IVerenigingBrolFeeder, VerenigingBrolFeeder>()
            .ConfigureRequestLocalization()
            .AddOpenTelemetry()
            .ConfigureProjectionsWithMarten(builder.Configuration)
            .ConfigureSwagger()
            .ConfigureElasticSearch(elasticSearchOptions)
            .AddMvc()
            .AddDataAnnotationsLocalization();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IApiControllerSpecification, ApiControllerSpec>());

        var app = builder.Build();

        app.MapPost(
            "/rebuild",
            async (IDocumentStore store, ILogger<Program> logger, CancellationToken cancellationToken) =>
            {
                var projectionDaemon = await store.BuildProjectionDaemonAsync();
                await projectionDaemon.RebuildProjection<PubliekVerenigingDetailProjection>(cancellationToken);
                logger.LogInformation("Rebuild complete");
            });

        app.SetUpSwagger();

        await DistributedLock<Program>.RunAsync(
            async () => await app.RunAsync(),
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
                "FirstChanceException event raised in {AppDomain}.",
                AppDomain.CurrentDomain.FriendlyName);

        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
            Log.Fatal(
                (Exception)eventArgs.ExceptionObject,
                "Encountered a fatal exception, exiting program.");
    }
}
