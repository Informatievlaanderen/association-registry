namespace AssociationRegistry.Public.Api;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DetailVerenigingen;
using Extensions;
using Infrastructure.Configuration;
using Infrastructure.Modules;
using S3;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Immutable;
using Constants;
using Infrastructure.Json;
using ListVerenigingen;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Locatie = ListVerenigingen.Locatie;
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
        var baseUrl = _configuration.GetValue<string>("BaseUrl");
        var baseUrlForExceptions = baseUrl.EndsWith("/")
            ? baseUrl.Substring(0, baseUrl.Length - 1)
            : baseUrl;

        var s3Options = new S3BlobClientOptions();
        _configuration.GetSection(nameof(S3BlobClientOptions)).Bind(s3Options);

        var organisationRegistryUri = _configuration.GetSection("OrganisationRegistryUri").Value;
        var associationRegistryUri = _configuration.GetSection("AssociationRegistryUri").Value;

        services.AddSingleton(_configuration.Get<AppSettings>());

        services.AddS3(_configuration);
        services.AddBlobClients(s3Options);
        services.AddDataCache();
        services.AddJsonLdContexts();
        services.AddSingleton<IVerenigingenRepository>(
            _ => new InMemoryVerenigingenRepository(
                new VerenigingListItem[]
                {
                    new(
                        "V1234567",
                        "FWA De vrolijke BA’s",
                        "DVB",
                        new Locatie("1770", "Liedekerke"),
                        ImmutableArray.Create(
                            new ListVerenigingActiviteit("Badminton", new Uri($"{associationRegistryUri}/V000010")),
                            new ListVerenigingActiviteit("Tennis", new Uri($"{associationRegistryUri}/V000010")))),
                },
                new VerenigingDetail[]
                {
                    new(
                        "V1234567",
                        "FWA De vrolijke BA’s",
                        "DVB",
                        "Korte omschrijving voor FWA De vrolijke BA's",
                        "Feitelijke vereniging",
                        DateOnly.FromDateTime(new DateTime(2022, 09, 15)),
                        DateOnly.FromDateTime(new DateTime(2023, 10, 16)),
                        new DetailVerenigingen.Locatie("1770", "Liedekerke"),
                        new ContactPersoon(
                            "Walter",
                            "Plop",
                            ImmutableArray.Create(
                                new ContactGegeven("email", "walter.plop@studio100.be"),
                                new ContactGegeven("telefoon", "100"))),
                        ImmutableArray.Create(new DetailVerenigingen.Locatie("1770", "Liedekerke")),
                        ImmutableArray.Create(
                            new DetailVerenigingActiviteit("Badminton", new Uri($"{associationRegistryUri}/V000010")),
                            new DetailVerenigingActiviteit("Tennis", new Uri($"{associationRegistryUri}/V000010"))),
                        ImmutableArray.Create(
                            new ContactGegeven("telefoon", "025462323"),
                            new ContactGegeven("email", "info@dotimeforyou.be"),
                            new ContactGegeven("website", "www.dotimeforyou.be"),
                            new ContactGegeven("socialmedia", "facebook/dotimeforyou"),
                            new ContactGegeven("socialmedia", "twitter/@dotimeforyou")
                        ),
                        DateOnly.FromDateTime(new DateTime(2022, 09, 26))),
                    new(
                        "V1234568",
                        "FWA De vrolijke BA’s",
                        String.Empty,
                        String.Empty,
                        String.Empty,
                        null,
                        null,
                        new DetailVerenigingen.Locatie("1840", "Londerzeel"),
                        null,
                        ImmutableArray.Create(new DetailVerenigingen.Locatie("1840", "Londerzeel")),
                        ImmutableArray<DetailVerenigingActiviteit>.Empty,
                        ImmutableArray<ContactGegeven>.Empty,
                        DateOnly.FromDateTime(new DateTime(2022, 09, 26))),
                }
            ));

        services
            .ConfigureDefaultForApi<Startup>(
                new StartupConfigureOptions
                {
                    Cors =
                    {
                        Origins = _configuration
                            .GetSection("Cors")
                            .GetChildren()
                            .Select(c => c.Value)
                            .ToArray(),
                    },
                    Server =
                    {
                        BaseUrl = baseUrlForExceptions,
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
                            Title = "Basisregisters Vlaanderen Verenigingenregister Public API",
                            Description = GetApiLeadingText(description),
                            Contact = new OpenApiContact
                            {
                                Name = "Digitaal Vlaanderen",
                                Email = "digitaal.vlaanderen@vlaanderen.be",
                                Url = new Uri("https://public.api.verenigingen.vlaanderen.be"),
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
                            var connectionStrings = _configuration
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
        //.Configure<ResponseOptions>(_configuration);

        var containerBuilder = new ContainerBuilder();
        containerBuilder.RegisterModule(new ApiModule(_configuration, services, _loggerFactory));
        _applicationContainer = containerBuilder.Build();

        return new AutofacServiceProvider(_applicationContainer);
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
                        Info = groupName => $"Basisregisters Vlaanderen - Verenigingenregister Public API {groupName}",
                        CSharpClientOptions =
                        {
                            ClassName = "Verenigingenregister",
                            Namespace = "Be.Vlaanderen.Basisregisters",
                        },
                        TypeScriptClientOptions =
                        {
                            ClassName = "Verenigingenregister",
                        },
                    },
                    MiddlewareHooks =
                    {
                        AfterMiddleware = x => x.UseMiddleware<AddNoCacheHeadersMiddleware>(),
                    },
                });
    }

    private static string GetApiLeadingText(ApiVersionDescription description)
        => $"Momenteel leest u de documentatie voor versie {description.ApiVersion} van de Basisregisters Vlaanderen Verenigingenregister Public API{string.Format(description.IsDeprecated ? ", **deze API versie is niet meer ondersteund * *." : ".")}";
}

public class ProblemJsonResponseFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var (_, value) in operation.Responses.Where(
                     entry =>
                         (entry.Key.StartsWith("4") && entry.Key != "400") ||
                         entry.Key.StartsWith("5")))
        {
            if (!value.Content.Any())
                return;

            var openApiMediaType = value.Content.First().Value;

            value.Content.Clear();
            value.Content.Add(
                new KeyValuePair<string, OpenApiMediaType>("application/problem+json", openApiMediaType));
        }
    }
}
