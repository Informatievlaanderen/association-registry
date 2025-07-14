namespace AssociationRegistry.Acm.Api;

using AcmBevraging;
using Asp.Versioning.ApplicationModels;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.Api.Localization;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Logging;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger.ReDoc;
using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Be.Vlaanderen.Basisregisters.Middleware.AddProblemJsonHeader;
using Constants;
using Destructurama;
using FluentValidation;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Infrastructure.Configuration;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Infrastructure.Metrics;
using JasperFx;
using Magda;
using Magda.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Oakton;
using OpenTelemetry.Extensions;
using Queries.VerenigingenPerInsz;
using Serilog;
using Serilog.Debugging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IExceptionHandler = Be.Vlaanderen.Basisregisters.Api.Exceptions.IExceptionHandler;

public class Program
{
    private const string AdminGlobalPolicyName = "Admin Global";
    public const string SuperAdminPolicyName = "Super Admin";

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = Directory.GetCurrentDirectory(),
                WebRootPath = "wwwroot",
            });

        LoadConfiguration(builder, args);

        SelfLog.Enable(Console.WriteLine);

        ConfigureEncoding();
        ConfigureJsonSerializerSettings();
        ConfigureAppDomainExceptions();

        ConfigereKestrel(builder);
        ConfigureLogger(builder);
        ConfigureWebHost(builder);
        ConfigureServices(builder);

        builder.Host.ApplyJasperFxExtensions();

        var app = builder.Build();

        // app.ConfigureAcmApiSwagger();

        app.UseRequestLocalization();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseReDoc(opt => { opt.RoutePrefix = "docs"; opt.SpecUrl = "/docs/v1/docs.json"; });

        GlobalStringLocalizer.Instance = new GlobalStringLocalizer(app.Services);

        app.AddProjectionEndpoints(
            app.Configuration.GetSection(RebuildConfigurationSection.SectionName).Get<RebuildConfigurationSection>()!);

        app
           .ConfigureDevelopmentEnvironment()
           .UseCors(StartupConstants.AllowSpecificOrigin);

        // Deze volgorde is belangrijk ! DKW
        app.UseMiddleware<ProblemDetailsMiddleware>();
        ConfigureExceptionHandler(app);
        ConfigureMiddleware(app);

        app.UseMiddleware<AddProblemJsonHeaderMiddleware>();

        ConfigureHealtChecks(app);
        ConfigureRequestLocalization(app);

        // Deze volgorde is belangrijk ! DKW
        app.UseRouting()
           .UseAuthentication()
           .UseAuthorization()
           .UseEndpoints(routeBuilder => routeBuilder.MapControllers());

        ConfigureLifetimeHooks(app);

        await app.RunJasperFxCommands(args);
    }

    private static void ConfigureRequestLocalization(WebApplication app)
    {
        var requestLocalizationOptions = app.Services
                                            .GetRequiredService<IOptions<RequestLocalizationOptions>>()
                                            .Value;

        app.UseRequestLocalization(requestLocalizationOptions);
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

    private static void ConfigureExceptionHandler(WebApplication app)
    {
        var problemDetailsHelper = app.Services.GetRequiredService<ProblemDetailsHelper>();
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger<ApiExceptionHandler>();

        var exceptionHandler = new ExceptionHandler(
            logger,
            Array.Empty<ApiProblemDetailsExceptionMapping>(),
            Array.Empty<IExceptionHandler>(),
            problemDetailsHelper);

        app.UseExceptionHandler404Allowed(
            b =>
            {
                b.UseCors(StartupConstants.AllowSpecificOrigin);

                b.UseMiddleware<ProblemDetailsMiddleware>();

                ConfigureMiddleware(b);

                var requestLocalizationOptions = app.Services
                                                    .GetRequiredService<IOptions<RequestLocalizationOptions>>()
                                                    .Value;

                b.UseRequestLocalization(requestLocalizationOptions);

                b
                   .Run(
                        async context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            context.Response.ContentType = MediaTypeNames.Application.Json;

                            var error = context.Features.Get<IExceptionHandlerFeature>();
                            var exception = error?.Error;

                            // Errors happening in the Apply() stuff result in an InvocationException due to the dynamic stuff.
                            if (exception is TargetInvocationException && exception.InnerException != null)
                                exception = exception.InnerException;

                            await exceptionHandler.HandleException(exception!, context);
                        });
            });
    }

    private static void LoadConfiguration(WebApplicationBuilder builder, params string[] args)
    {
        builder.Configuration
               .AddJsonFile("appsettings.json")
               .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName.ToLowerInvariant()}.json", optional: true,
                            reloadOnChange: false)
               .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
               .AddEnvironmentVariables()
               .AddCommandLine(args)
               .AddInMemoryCollection();
    }

    private static void ConfigureMiddleware(IApplicationBuilder app)
    {
        app
           .UseMiddleware<EnableRequestRewindMiddleware>()
           .UseMiddleware<AddCorrelationIdToResponseMiddleware>()
           .UseMiddleware<AddCorrelationIdMiddleware>()
           .UseMiddleware<AddCorrelationIdToLogContextMiddleware>()
           .UseMiddleware<AddHttpSecurityHeadersMiddleware>()
           .UseMiddleware<AddRemoteIpAddressMiddleware>(AddRemoteIpAddressMiddleware.UrnBasisregistersVlaanderenIp)
           .UseMiddleware<AddVersionHeaderMiddleware>(AddVersionHeaderMiddleware.HeaderName)
           .UseMiddleware<AddNoCacheHeadersMiddleware>()
           .UseMiddleware<DefaultResponseCompressionQualityMiddleware>(
                new Dictionary<string, double>
                {
                    { "br", 1.0 },
                    { "gzip", 0.9 },
                })
           .UseResponseCompression();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var postgreSqlOptionsSection = builder.Configuration.GetPostgreSqlOptionsSection();
        var appSettings = builder.Configuration.Get<AppSettings>();

        builder.ConfigureOpenTelemetry(new AcmInstrumentation());

        builder.Services
               .AddSingleton(appSettings)
               .AddMarten(postgreSqlOptionsSection, builder.Configuration)
               .AddHttpContextAccessor()
               .AddControllers();


        builder.            Services
                           .AddLocalization(cfg => cfg.ResourcesPath = "Resources")
                           .AddSingleton<IStringLocalizerFactory, SharedStringLocalizerFactory<StartupDefaults.DefaultResources>>()
                           .AddSingleton<ResourceManagerStringLocalizerFactory, ResourceManagerStringLocalizerFactory>()

                           .Configure<RequestLocalizationOptions>(opts =>
                            {
                                const string fallbackCulture = "en-GB";
                                var defaultRequestCulture = new RequestCulture( new CultureInfo(fallbackCulture));
                                var supportedCulturesOrDefault =  new[] { new CultureInfo(fallbackCulture) };

                                opts.DefaultRequestCulture = defaultRequestCulture;
                                opts.SupportedCultures = supportedCulturesOrDefault;
                                opts.SupportedUICultures = supportedCulturesOrDefault;

                                opts.FallBackToParentCultures = true;
                                opts.FallBackToParentUICultures = true;
                            })

                           .Configure<RequestLocalizationOptions>(opts =>
                            {
                                const string fallbackCulture = "en-GB";
                                var defaultRequestCulture = new RequestCulture(new CultureInfo(fallbackCulture));
                                var supportedCulturesOrDefault = new[] { new CultureInfo(fallbackCulture) };

                                opts.DefaultRequestCulture = defaultRequestCulture;
                                opts.SupportedCultures = supportedCulturesOrDefault;
                                opts.SupportedUICultures = supportedCulturesOrDefault;

                                opts.FallBackToParentCultures = true;
                                opts.FallBackToParentUICultures = true;
                            });

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IApiControllerSpecification, ApiControllerSpec>());

        builder.Services
               .AddSingleton(
                    new StartupConfigureOptions
                    {
                        Server =
                        {
                            BaseUrl = builder.Configuration.GetBaseUrl(),
                        },
                    })
               .ConfigureOptions<ProblemDetailsSetup>()
               .Configure<Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetailsOptions>(
                    cfg =>
                    {
                        foreach (var header in StartupConstants.ExposedHeaders)
                        {
                            cfg.AllowedHeaderNames.Add(header);
                        }
                    })
               .TryAddEnumerable(ServiceDescriptor
                                    .Transient<IConfigureOptions<Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetailsOptions>,
                                         ProblemDetailsOptionsSetup>());

        builder.Services
               .AddMvcCore(
                    cfg =>
                    {
                        cfg.RespectBrowserAcceptHeader = false;
                        cfg.ReturnHttpNotAcceptable = true;

                        cfg.Filters.Add(new LoggingFilterFactory(StartupConstants.HttpMethodsAsString));

                        cfg.Filters.Add<OperationCancelledExceptionFilterAttribute>();

                        cfg.EnableEndpointRouting = false;
                    })
               .AddCors(
                    cfg =>
                    {
                        cfg.AddPolicy(
                            StartupConstants.AllowAnyOrigin,
                            configurePolicy: corsPolicy => corsPolicy
                                                          .AllowAnyOrigin()
                                                          .WithMethods(StartupConstants.HttpMethodsAsString)
                                                          .WithHeaders(StartupConstants.Headers)
                                                          .WithExposedHeaders(StartupConstants.ExposedHeaders)
                                                          .SetPreflightMaxAge(TimeSpan.FromSeconds(60 * 15)));

                        cfg.AddPolicy(
                            StartupConstants.AllowSpecificOrigin,
                            configurePolicy: corsPolicy => corsPolicy
                                                          .WithOrigins(builder.Configuration.GetValue<string[]>("Cors") ??
                                                                       Array.Empty<string>())
                                                          .WithMethods(StartupConstants.HttpMethodsAsString)
                                                          .WithHeaders(StartupConstants.Headers)
                                                          .WithExposedHeaders(StartupConstants.ExposedHeaders)
                                                          .SetPreflightMaxAge(TimeSpan.FromSeconds(60 * 15))
                                                          .AllowCredentials());
                    })
               .AddControllersAsServices()
               .AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                                           .RequireClaim(Security.ClaimTypes.Scope, Security.Scopes.ACM)
                                           .Build();

                    options.AddPolicy(
                        SuperAdminPolicyName,
                        new AuthorizationPolicyBuilder()
                           .RequireClaim(Security.ClaimTypes.Scope, Security.Scopes.Admin)
                           .RequireClaim(Security.ClaimTypes.ClientId, appSettings.SuperAdminClientIds)
                           .Build());
                })
               .AddNewtonsoftJson(
                    opt =>
                    {
                        opt.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                        opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    })
               .AddXmlDataContractSerializerFormatters()
               .AddFormatterMappings()
               .AddApiExplorer();

        var oAuth2IntrospectionOptions = builder.Configuration.GetSection(nameof(OAuth2IntrospectionOptions))
                                   .Get<OAuth2IntrospectionOptions>()!;

        builder.Services.AddSingleton(oAuth2IntrospectionOptions);
        builder.Services.AddAuthentication(
                    options =>
                    {
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
               .AddOAuth2Introspection(
                    JwtBearerDefaults.AuthenticationScheme,
                    configureOptions: options =>
                    {
                        // var oAuth2IntrospectionOptions = builder.Configuration.GetSection(nameof(OAuth2IntrospectionOptions))
                        //                                         .Get<OAuth2IntrospectionOptions>()!;

                        options.ClientId = oAuth2IntrospectionOptions.ClientId;
                        options.ClientSecret = oAuth2IntrospectionOptions.ClientSecret;
                        options.Authority = oAuth2IntrospectionOptions.Authority;
                        options.IntrospectionEndpoint = oAuth2IntrospectionOptions.IntrospectionEndpoint;
                    }
                );

        builder.Services
               .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
               .AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddHealthChecks();

        builder.Services
               .AddLocalization(cfg => cfg.ResourcesPath = "Resources")
               .AddSingleton<IStringLocalizerFactory, SharedStringLocalizerFactory<StartupDefaults.DefaultResources>>()
               .AddSingleton<ResourceManagerStringLocalizerFactory, ResourceManagerStringLocalizerFactory>()
               .Configure<RequestLocalizationOptions>(
                    opts =>
                    {
                        opts.DefaultRequestCulture = new RequestCulture(new CultureInfo(StartupConstants.Culture));
                        opts.SupportedCultures = new[] { new CultureInfo(StartupConstants.Culture) };
                        opts.SupportedUICultures = new[] { new CultureInfo(StartupConstants.Culture) };

                        opts.FallBackToParentCultures = true;
                        opts.FallBackToParentUICultures = true;
                    });

        builder.Services
               .AddApiVersioning(
                    cfg =>
                    {
                        cfg.ReportApiVersions = true;
                        // TODO
                        // cfg.ErrorResponses = new ProblemDetailsResponseProvider();
                    })
               .AddApiExplorer(
                    cfg =>
                    {
                        cfg.GroupNameFormat = "'v'VVV";
                        cfg.SubstituteApiVersionInUrl = true;
                    });

        builder.Services
               .AddEndpointsApiExplorer()
               .AddResponseCompression(
                    cfg =>
                    {
                        cfg.EnableForHttps = true;

                        cfg.Providers.Add<BrotliCompressionProvider>();
                        cfg.Providers.Add<GzipCompressionProvider>();

                        cfg.MimeTypes = new[]
                        {
                            // General
                            "text/plain",
                            "text/csv",

                            // Static files
                            "text/css",
                            "application/javascript",

                            // MVC
                            "text/html",
                            "application/xml",
                            "text/xml",
                            "application/json",
                            "text/json",
                            "application/ld+json",
                            "application/atom+xml",

                            // Fonts
                            "application/font-woff",
                            "font/otf",
                            "application/vnd.ms-fontobject",
                        };
                    })
               .Configure<GzipCompressionProviderOptions>(cfg => cfg.Level = CompressionLevel.Fastest)
               .Configure<BrotliCompressionProviderOptions>(cfg => cfg.Level = CompressionLevel.Fastest)
               .Configure<KestrelServerOptions>(serverOptions => serverOptions.AllowSynchronousIO = true);

        builder.Services.AddAcmApiSwagger(appSettings);


        builder.Services
               .AddTransient<IVerenigingenPerInszQuery, VerenigingenPerInszQuery>()
               .AddTransient<IVerenigingenPerKboNummerService, VerenigingenPerKboNummerService>()
               .AddTransient<IRechtsvormCodeService, RechtsvormCodeService>();

        builder.Services.AddSingleton<ProblemDetailsHelper>();
    }

    private static void ConfigureWebHost(WebApplicationBuilder builder)
        => builder.WebHost.CaptureStartupErrors(captureStartupErrors: true);

    private static void ConfigureLogger(WebApplicationBuilder builder)
    {
        var loggerConfig =
            new LoggerConfiguration()
               .ReadFrom.Configuration(builder.Configuration)
               .Enrich.FromLogContext()
               .Enrich.WithMachineName()
               .Enrich.WithThreadId()
               .Enrich.WithEnvironmentUserName()
               .Destructure.JsonNetTypes();

        var logger = loggerConfig.CreateLogger();

        Log.Logger = logger;

        builder.Logging
                //.AddSerilog(logger)
               .AddOpenTelemetry();
    }

    private static void RunWithLock<T>(IWebHostBuilder webHostBuilder) where T : class
    {
        var webHost = webHostBuilder.Build();
        var services = webHost.Services;
        var logger = services.GetRequiredService<ILogger<T>>();

        DistributedLock<T>.Run(
            runFunc: () => webHost.Run(),
            DistributedLockOptions.Defaults,
            logger);
    }

    private static void ConfigereKestrel(WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(
            options =>
            {
                options.AddServerHeader = false;

                options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(value: 120);

                options.Listen(
                    new IPEndPoint(IPAddress.Any, port: 11002),
                    configure: listenOptions =>
                    {
                        listenOptions.UseConnectionLogging();
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    });
            });
    }

    private static void ConfigureEncoding()
        => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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

    private static void ConfigureLifetimeHooks(WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() => Log.Information("ACM Api started"));

        app.Lifetime.ApplicationStopping.Register(
            () =>
            {
                Log.Information("ACM Api stopping");
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
