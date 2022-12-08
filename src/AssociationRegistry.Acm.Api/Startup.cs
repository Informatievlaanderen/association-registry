namespace AssociationRegistry.Acm.Api;

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Extensions;
using Infrastructure.Configuration;
using Infrastructure.Modules;
using Infrastructure.Options;
using S3;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Be.Vlaanderen.Basisregisters.Api;
using Constants;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

/// <summary>Represents the startup process for the application.</summary>
public class Startup
{
    private const string DatabaseTag = "db";

    private IContainer _applicationContainer = null!;

    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
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

        services.AddS3(_configuration);
        services.AddBlobClients(s3Options);
        services.AddDataCache();
        services.AddAuthentication(
                options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddOAuth2Introspection(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    var configOptions = _configuration.GetSection(nameof(OAuth2IntrospectionOptions)).Get<OAuth2IntrospectionOptions>();
                    options.ClientId = configOptions.ClientId;
                    options.ClientSecret = configOptions.ClientSecret;
                    options.Authority = configOptions.Authority;
                    options.IntrospectionEndpoint = configOptions.IntrospectionEndpoint;
                }
            );
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
                        ApiInfo = (_, description) => new OpenApiInfo
                        {
                            Version = description.ApiVersion.ToString(),
                            Title = "Basisregisters Vlaanderen Verenigingsregister ACM API",
                            Description = GetApiLeadingText(description),
                            Contact = new OpenApiContact
                            {
                                Name = "Digitaal Vlaanderen",
                                Email = "digitaal.vlaanderen@vlaanderen.be",
                                Url = new Uri("https://acm.verenigingen.vlaanderen.be"),
                            },
                        },
                        XmlCommentPaths = new[] { typeof(Startup).GetTypeInfo().Assembly.GetName().Name! },
                    },
                    MiddlewareHooks =
                    {
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
                        Authorization = options =>
                        {
                            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                                .RequireClaim(Security.ClaimTypes.Scope, Security.Scopes.ACM)
                                .Build();
                        },
                    },
                })
            .Configure<ResponseOptions>(_configuration);

        var containerBuilder = new ContainerBuilder();
        containerBuilder.RegisterModule(new ApiModule(
            //_configuration,
            services
            //, _loggerFactory
            ));
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
                        Info = groupName => $"Basisregisters Vlaanderen - Verenigingsregister ACM API {groupName}",
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
        => $"Momenteel leest u de documentatie voor versie {description.ApiVersion} van de Basisregisters Vlaanderen Verenigingsregister ACM API{string.Format(description.IsDeprecated ? ", **deze API versie is niet meer ondersteund * *." : ".")}";
}
