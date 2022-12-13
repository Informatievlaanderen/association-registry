namespace AssociationRegistry.Public.Api;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Be.Vlaanderen.Basisregisters.Api;
using ConfigurationBindings;
using Constants;
using Extensions;
using Infrastructure.Configuration;
using Infrastructure.Json;
using Infrastructure.Modules;
using ListVerenigingen;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Projections;
using S3;
using SearchVerenigingen;
using static ConfigHelpers;
using ListVerenigingActiviteit = ListVerenigingen.Activiteit;
using DetailVerenigingActiviteit = DetailVerenigingen.Activiteit;

/// <summary>Represents the startup process for the application.</summary>
public class Startup
{
    private const string DatabaseTag = "db";

    private IContainer _applicationContainer = null!;

    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    public Startup(
        IConfiguration configuration,
        ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }

    /// <summary>Configures services for the application.</summary>
    /// <param name="services">The collection of services to configure the application with.</param>
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        var postgreSqlOptions = _configuration.GetSection(PostgreSqlOptionsSection.Name)
            .Get<PostgreSqlOptionsSection>();

        ThrowIfInvalidPostgreSqlOptions(postgreSqlOptions);

        var elasticSearchOptions = _configuration.GetSection("ElasticClientOptions")
            .Get<ElasticSearchOptionsSection>();

        ThrowIfInvalidElasticOptions(elasticSearchOptions);

        var s3Options = new S3BlobClientOptions();
        _configuration.GetSection(nameof(S3BlobClientOptions)).Bind(s3Options);

        var appSettings = _configuration.Get<AppSettings>();
        services.AddSingleton(appSettings);

        var assembly = typeof(Startup).Assembly;
        var serviceName = assembly.FullName;
        var serviceVersion = "1.0.0";
        var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
        Action<ResourceBuilder> configureResource = r => r.AddService(
                serviceName, serviceVersion: assemblyVersion, serviceInstanceId: Environment.MachineName)
            .AddAttributes(
                new Dictionary<string, object>()
                {
                    ["deployment.environment"] =
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant() ?? "unknown",
                });


        services.AddOpenTelemetryTracing(b =>
        {
            b
                .AddSource(serviceName)
                .ConfigureResource(configureResource).AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation(
                    options =>
                    {
                        options.EnrichWithHttpRequest =
                            (activity, request) => activity.SetParentId(request.Headers["traceparent"]);
                        options.Filter = context => context.Request.Method != HttpMethods.Options;
                    })
                // .AddSqlClientInstrumentation(options => { options.SetDbStatementForText = true; })
                .AddOtlpExporter(
                    options =>
                    {
                        options.Protocol = OtlpExportProtocol.Grpc;
                        options.Endpoint = new Uri("http://localhost:4317");
                    });
            // .AddConsoleExporter();
        });

        services.AddLogging(
            builder =>
            {
                builder.ClearProviders();
                builder.AddOpenTelemetry(
                    options =>
                    {
                        options.ConfigureResource(configureResource);

                        options.IncludeScopes = true;
                        options.IncludeFormattedMessage = true;
                        options.ParseStateValues = true;

                        options.AddOtlpExporter(
                                exporter =>
                                {
                                    exporter.Protocol = OtlpExportProtocol.Grpc;
                                    exporter.Endpoint = new Uri("http://localhost:4317");
                                })
                            .AddConsoleExporter();
                    });
            });

        services.AddOpenTelemetryMetrics(options =>
        {
            options.ConfigureResource(configureResource)
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation();

            options.AddOtlpExporter(exporter =>
            {
                exporter.Protocol = OtlpExportProtocol.Grpc;
                exporter.Endpoint = new Uri("http://localhost:4317");
            });
            // options.AddConsoleExporter();
        });

        services.AddElasticSearch(elasticSearchOptions);

        services.AddTransient<IElasticRepository, ElasticRepository>();
        services.AddSingleton<IVerenigingBrolFeeder, VerenigingBrolFeeder>();

        services.RegisterDomainEventHandlers(GetType().Assembly);

        services.AddMarten(postgreSqlOptions, _configuration);

        services.AddS3(_configuration);
        services.AddBlobClients(s3Options);
        services.AddDataCache();
        services.AddJsonLdContexts();

        // todo: remove when static version of detail api is removed
        services.AddSingleton<IVerenigingenRepository>(_ => InMemoryVerenigingenRepository.Create(appSettings.BaseUrl));

        services.AddSingleton<SearchVerenigingenMapper>();

        AddSwagger(services);

        ConfigureDefaultsForApi(services, _configuration);

        var containerBuilder = new ContainerBuilder();
        containerBuilder.RegisterModule(new ApiModule(_configuration, services, _loggerFactory));
        _applicationContainer = containerBuilder.Build();

        return new AutofacServiceProvider(_applicationContainer);
    }

    private static void ConfigureDefaultsForApi(IServiceCollection services, IConfiguration configuration)
    {
        services
            .ConfigureDefaultForApi<Startup>(
                new StartupConfigureOptions
                {
                    Cors =
                    {
                        Origins = configuration
                            .GetSection("Cors")
                            .GetChildren()
                            .Select(c => c.Value)
                            .ToArray(),
                    },
                    Server =
                    {
                        BaseUrl = GetBaseUrlForExceptions(configuration),
                    },
                    Swagger =
                    {
                        MiddlewareHooks =
                        {
                            AfterSwaggerGen = options => { options.CustomSchemaIds(type => type.ToString()); },
                        },
                        ApiInfo = (_, description) => new OpenApiInfo
                        {
                            Version = description.ApiVersion.ToString(),
                            Title = "Basisregisters Vlaanderen Verenigingsregister Publieke API",
                            Description = GetApiLeadingText(description),
                            Contact = new OpenApiContact
                            {
                                Name = "Digitaal Vlaanderen",
                                Email = "digitaal.vlaanderen@vlaanderen.be",
                                Url = new Uri("https://publiek.verenigingen.vlaanderen.be"),
                            },
                        },
                        XmlCommentPaths = new[] { typeof(Startup).GetTypeInfo().Assembly.GetName().Name! },
                    },
                    MiddlewareHooks =
                    {
                        ConfigureJsonOptions = options =>
                        {
                            options.SerializerSettings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                            options.SerializerSettings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
                            options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        },
                        // AfterHealthChecks = health =>
                        // {
                        //     var connectionStrings = configuration
                        //         .GetSection("ConnectionStrings")
                        //         .GetChildren();
                        //     //
                        //     // foreach (var connectionString in connectionStrings)
                        //     //     health.AddSqlServer(
                        //     //         connectionString.Value,
                        //     //         name: $"sqlserver-{connectionString.Key.ToLowerInvariant()}",
                        //     //         tags: new[] { DatabaseTag, "sql", "sqlserver" });
                        //
                        //     // health.AddDbContextCheck<LegacyContext>(
                        //     //     $"dbcontext-{nameof(LegacyContext).ToLowerInvariant()}",
                        //     //     tags: new[] { DatabaseTag, "sql", "sqlserver" });
                        // },
                    },
                });
    }

    private static string GetBaseUrlForExceptions(IConfiguration configuration)
        => TrimTrailingSlash(configuration.GetValue<string>("BaseUrl"));

    private static string TrimTrailingSlash(string baseUrl)
        => baseUrl.EndsWith("/")
            ? baseUrl[..^1]
            : baseUrl;

    private static void AddSwagger(IServiceCollection services)
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

    public void Configure(
        IServiceProvider serviceProvider,
        IApplicationBuilder app,
        IWebHostEnvironment env,
        IHostApplicationLifetime appLifetime,
        ILoggerFactory loggerFactory,
        IApiVersionDescriptionProvider apiVersionProvider
        // ApiDataDogToggle datadogToggle,
        // ApiDebugDataDogToggle debugDataDogToggle,
        // HealthCheckService healthCheckService
    )
    {
        // StartupHelpers.CheckDatabases(healthCheckService, DatabaseTag, loggerFactory).GetAwaiter().GetResult();

        app
            // .UseDataDog<Startup>(new DataDogOptions
            // {
            //     Common =
            //     {
            //         ServiceProvider = serviceProvider,
            //         LoggerFactory = loggerFactory
            //     },
            //     Toggles =
            //     {
            //         Enable = datadogToggle,
            //         Debug = debugDataDogToggle
            //     },
            //     Tracing =
            //     {
            //         ServiceName = _configuration["DataDog:ServiceName"],
            //     }
            // })
            .UseDefaultForApi(
                new StartupUseOptions
                {
                    Common =
                    {
                        ApplicationContainer = _applicationContainer,
                        ServiceProvider = serviceProvider,
                        HostingEnvironment = env,
                        ApplicationLifetime = appLifetime,
                        LoggerFactory = loggerFactory,
                    },
                    Api =
                    {
                        VersionProvider = apiVersionProvider,
                        Info = groupName => $"Basisregisters Vlaanderen - Verenigingsregister Publieke API {groupName}",
                        CSharpClientOptions =
                        {
                            ClassName = "Verenigingsregister",
                            Namespace = "Be.Vlaanderen.Basisregisters",
                        },
                        TypeScriptClientOptions =
                        {
                            ClassName = "Verenigingsregister",
                        },
                    },
                    MiddlewareHooks =
                    {
                        AfterMiddleware = x => x.UseMiddleware<AddNoCacheHeadersMiddleware>(),
                    },
                });
    }

    private static string GetApiLeadingText(ApiVersionDescription description)
        => $"Momenteel leest u de documentatie voor versie {description.ApiVersion} van de Basisregisters Vlaanderen Verenigingsregister Publieke API{string.Format(description.IsDeprecated ? ", **deze API versie is niet meer ondersteund * *." : ".")}";
}
