namespace AssociationRegistry.Public.Api;

using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using ConfigurationBindings;
using Constants;
using Extensions;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Configuration;
using Infrastructure.Json;
using Infrastructure.Modules;
using ListVerenigingen;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Projections;
using S3;
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
        var postgreSqlOptions = _configuration.GetSection("PostgreSQLOptions")
            .Get<PostgreSqlOptionsSection>();

        ThrowIfInvalidPostgreSqlOptions(postgreSqlOptions);

        var elasticSearchOptions = _configuration.GetSection("ElasticClientOptions")
            .Get<ElasticSearchOptionsSection>();

        ThrowIfInvalidElasticOptions(elasticSearchOptions);

        var s3Options = new S3BlobClientOptions();
        _configuration.GetSection(nameof(S3BlobClientOptions)).Bind(s3Options);

        var organisationRegistryUri = _configuration.GetSection("OrganisationRegistryUri").Value;
        var associationRegistryUri = _configuration.GetSection("AssociationRegistryUri").Value;

        services.AddSingleton(_configuration.Get<AppSettings>());

        services.AddElasticSearch(elasticSearchOptions);

        services.AddTransient<IElasticRepository, ElasticRepository>();
        services.AddSingleton<IVerenigingBrolFeeder, VerenigingBrolFeeder>();

        services.RegisterDomainEventHandlers(GetType().Assembly);

        AddMarten(services, postgreSqlOptions, _configuration);

        services.AddS3(_configuration);
        services.AddBlobClients(s3Options);
        services.AddDataCache();
        services.AddJsonLdContexts();

        // todo: remove when detail api works with real projection
        services.AddSingleton<IVerenigingenRepository>(_ => InMemoryVerenigingenRepository.Create(associationRegistryUri));

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
                                Url = new Uri("https://public-api.verenigingen.vlaanderen.be"),
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
                        AfterHealthChecks = health =>
                        {
                            var connectionStrings = configuration
                                .GetSection("ConnectionStrings")
                                .GetChildren();

                            foreach (var connectionString in connectionStrings)
                                health.AddSqlServer(
                                    connectionString.Value,
                                    name: $"sqlserver-{connectionString.Key.ToLowerInvariant()}",
                                    tags: new[] { DatabaseTag, "sql", "sqlserver" });

                            // health.AddDbContextCheck<LegacyContext>(
                            //     $"dbcontext-{nameof(LegacyContext).ToLowerInvariant()}",
                            //     tags: new[] { DatabaseTag, "sql", "sqlserver" });
                        },
                    },
                });
    }

    private static string GetBaseUrlForExceptions(IConfiguration configuration)
        => TrimTrailingSlash(configuration.GetValue<string>("BaseUrl"));

    private static string TrimTrailingSlash(string baseUrl)
        => baseUrl.EndsWith("/")
            ? baseUrl[..^1]
            : baseUrl;

    private static void AddMarten(IServiceCollection services, PostgreSqlOptionsSection postgreSqlOptions, IConfiguration configuration)
    {
        var martenConfiguration = services.AddMarten(
            serviceProvider =>
            {
                var connectionString1 = GetPostgresConnectionString(postgreSqlOptions);

                var opts = new StoreOptions();

                opts.Connection(connectionString1);

                opts.Events.StreamIdentity = StreamIdentity.AsString;

                opts.Projections.Add(
                    new MartenSubscription(
                        new MartenEventsConsumer(serviceProvider)),
                    ProjectionLifecycle.Async);

                opts.AddPostgresProjections();

                return opts;
            });

        if (configuration["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.AddAsyncDaemon(DaemonMode.Solo);
    }

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
