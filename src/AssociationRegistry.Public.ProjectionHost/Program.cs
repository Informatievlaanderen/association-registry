using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AssociationRegistry.OpenTelemetry.Extensions;
using AssociationRegistry.Public.ProjectionHost;
using AssociationRegistry.Public.ProjectionHost.ConfigurationBindings;
using AssociationRegistry.Public.ProjectionHost.Constants;
using AssociationRegistry.Public.ProjectionHost.Extensions;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Json;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Destructurama;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Marten.Services;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using Serilog.Debugging;
using ApiControllerSpec = AssociationRegistry.Public.ProjectionHost.ApiControllerSpec;

var builder = WebApplication.CreateBuilder(args);

SelfLog.Enable(Console.WriteLine);

ConfigureEncoding();
ConfigureJsonSerializerSettings();
ConfigureAppDomainExceptions();

var environment = builder.Environment.EnvironmentName;

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

GetValidPostgreSqlOptionsOrThrow(builder);
var elasticSearchOptions = GetValidElasticSearchOptionsOrThrow(builder);

//ConfigureKestrel(builder, environment);

builder.Services.RegisterDomainEventHandlers(typeof(Program).Assembly);

builder.Services
    .AddTransient<IElasticRepository, ElasticRepository>()
    .AddSingleton<IVerenigingBrolFeeder, VerenigingBrolFeeder>();

var martenConfiguration = AddMarten(builder);

if (builder.Configuration["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
    martenConfiguration.AddAsyncDaemon(DaemonMode.Solo);

builder.Services.AddOpenTelemetry();
builder.Services.AddElasticSearch(elasticSearchOptions);
builder.Services.AddMvc();

AddSwagger(builder.Services);

builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IApiControllerSpecification, ApiControllerSpec>());

// builder.Services.AddSingleton(
//     new StartupConfigureOptions()
//     {
//         Server =
//         {
//             BaseUrl = GetBaseUrlForExceptions(builder.Configuration),
//         },
//     });
// builder.Services.AddScoped<ProblemDetailsSetup>();
// builder.Services.AddScoped<ProblemDetailsHelper>();
// builder.Services.AddHttpContextAccessor()
//     .ConfigureOptions<ProblemDetailsSetup>()
//     .AddProblemDetails();

var app = builder.Build();

await DistributedLock<Program>.RunAsync(
    async () => { app.Run(); },
    DistributedLockOptions.LoadFromConfiguration(builder.Configuration),
    NullLogger.Instance);

static void AddSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(
        options =>
        {
            options.MapType<DateOnly>(
                () => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date",
                    Pattern = "yyyy-MM-dd",
                });
        });
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
    AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
        Log.Debug(
            eventArgs.Exception,
            "FirstChanceException event raised in {AppDomain}.",
            AppDomain.CurrentDomain.FriendlyName);

    AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
        Log.Fatal(
            (Exception)eventArgs.ExceptionObject,
            "Encountered a fatal exception, exiting program.");
}

void ConfigureKestrel(WebApplicationBuilder webApplicationBuilder, string s)
{
    static void AddListener(
        KestrelServerOptions options,
        int port,
        X509Certificate2? certificate)
    {
        options.Listen(
            new IPEndPoint(IPAddress.Any, port),
            listenOptions =>
            {
                listenOptions.UseConnectionLogging();

                if (certificate == null)
                    return;

                listenOptions.UseHttps(certificate);
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });
    }

    static void AddDevelopmentPorts(
        KestrelServerOptions options,
        int? httpPort,
        int? httpsPort,
        X509Certificate2? certificate)
    {
        if (httpPort.HasValue)
            AddListener(options, httpPort.Value, certificate: null);

        if (httpsPort.HasValue && certificate != null)
            AddListener(options, httpsPort.Value, certificate);
    }

    webApplicationBuilder.WebHost.UseKestrel(
            x =>
            {
                x.AddServerHeader = false;

                // Needs to be bigger than traefik timeout, which is 90seconds
                // https://github.com/containous/traefik/issues/3237
                x.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(value: 120);

                if (s == "Development")
                    AddDevelopmentPorts(
                        x,
                        httpPort: 11005,
                        httpsPort: null,
                        certificate: null);
            }
        ).UseSockets()
        .CaptureStartupErrors(captureStartupErrors: true)
        .UseContentRoot(Directory.GetCurrentDirectory())
        .ConfigureAppConfiguration(
            (hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

                config
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .AddJsonFile($"appsettings.{env.EnvironmentName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                    .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                    .AddEnvironmentVariables()
                    .AddCommandLine(Array.Empty<string>());
            })
        .ConfigureLogging(
            (hostingContext, logging) =>
            {
                var loggerConfiguration = new LoggerConfiguration()
                    .ReadFrom.Configuration(hostingContext.Configuration);

                loggerConfiguration = loggerConfiguration
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithEnvironmentUserName()
                    .Destructure.JsonNetTypes();

                var logger = Log.Logger = loggerConfiguration.CreateLogger();

                logging.AddSerilog(logger);
            });
}

MartenServiceCollectionExtensions.MartenConfigurationExpression AddMarten(WebApplicationBuilder builder1)
{
    static string GetPostgresConnectionString(PostgreSqlOptionsSection postgreSqlOptions)
    {
        return $"host={postgreSqlOptions.Host};" +
               $"database={postgreSqlOptions.Database};" +
               $"password={postgreSqlOptions.Password};" +
               $"username={postgreSqlOptions.Username}";
    }

    static JsonNetSerializer CreateCustomMartenSerializer()
    {
        var jsonNetSerializer = new JsonNetSerializer();
        jsonNetSerializer.Customize(
            s =>
            {
                s.DateParseHandling = DateParseHandling.None;
                s.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                s.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
            });
        return jsonNetSerializer;
    }

    var martenConfigurationExpression = builder1.Services.AddMarten(
        serviceProvider =>
        {
            var postgreSqlOptions = builder1.Configuration.GetSection(PostgreSqlOptionsSection.Name)
                .Get<PostgreSqlOptionsSection>();
            var connectionString = GetPostgresConnectionString(postgreSqlOptions);

            var opts = new StoreOptions();

            opts.Connection(connectionString);

            opts.Events.StreamIdentity = StreamIdentity.AsString;

            opts.Events.MetadataConfig.EnableAll();

            opts.Projections.Add<VerenigingDetailProjection>();
            opts.Projections.Add(
                new MartenSubscription(
                    new MartenEventsConsumer(builder1.Services.BuildServiceProvider())),
                ProjectionLifecycle.Async);

            opts.Serializer(CreateCustomMartenSerializer());
            return opts;
        });
    return martenConfigurationExpression;
}

PostgreSqlOptionsSection GetValidPostgreSqlOptionsOrThrow(WebApplicationBuilder webApplicationBuilder1)
{
    var postgreSqlOptions = webApplicationBuilder1.Configuration.GetSection(PostgreSqlOptionsSection.Name)
        .Get<PostgreSqlOptionsSection>();

    ConfigHelpers.ThrowIfInvalidPostgreSqlOptions(postgreSqlOptions);
    return postgreSqlOptions;
}

ElasticSearchOptionsSection GetValidElasticSearchOptionsOrThrow(WebApplicationBuilder builder2)
{
    var elasticSearchOptionsSection = builder2.Configuration.GetSection("ElasticClientOptions")
        .Get<ElasticSearchOptionsSection>();

    ConfigHelpers.ThrowIfInvalidElasticOptions(elasticSearchOptionsSection);
    return elasticSearchOptionsSection;
}

static string GetBaseUrlForExceptions(IConfiguration configuration)
    => configuration.GetValue<string>("BaseUrl").TrimEnd('/');
