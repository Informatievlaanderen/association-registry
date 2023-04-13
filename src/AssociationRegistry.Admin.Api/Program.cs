namespace AssociationRegistry.Admin.Api;

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
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.Api.Localization;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Logging;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Be.Vlaanderen.Basisregisters.Middleware.AddProblemJsonHeader;
using Constants;
using Destructurama;
using DuplicateDetection;
using DuplicateVerenigingDetection;
using EventStore;
using FluentValidation;
using Framework;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Infrastructure;
using Infrastructure.Configuration;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Infrastructure.Json;
using Magda;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OpenTelemetry.Extensions;
using Serilog;
using Serilog.Debugging;
using VCodeGeneration;
using Vereniging;
using Wolverine;

public class Program
{
    private const string AdminGlobalPolicyName = "Admin Global";

    public static void Main(string[] args)
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

        builder.Host.UseWolverine(
            options => options.Handlers.Discovery(
                source => source.IncludeAssembly(typeof(Vereniging).Assembly)));

        var app = builder.Build();

        GlobalStringLocalizer.Instance = new GlobalStringLocalizer(app.Services);

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
        app.ConfigureAdminApiSwagger();

        // Deze volgorde is belangrijk ! DKW
        app.UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(routeBuilder => routeBuilder.MapControllers().RequireAuthorization(AdminGlobalPolicyName));

        ConfigureLifetimeHooks(app);

        app.Run();
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

    private static void ConfigureExceptionHandler(WebApplication app)
    {
        var problemDetailsHelper = app.Services.GetRequiredService<ProblemDetailsHelper>();
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger<ApiExceptionHandler>();
        var exceptionHandler = new ExceptionHandler(
            logger,
            Array.Empty<ApiProblemDetailsExceptionMapping>(),
            new IExceptionHandler[]
            {
                new BadHttpRequestExceptionHandler(problemDetailsHelper),
                new UnknownEntityExceptionHandler(problemDetailsHelper),
                new CouldNotParseRequestExceptionHandler(problemDetailsHelper),
                new JsonReaderExceptionHandler(problemDetailsHelper),
            },
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
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
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

        builder.Services
            .AddSingleton(postgreSqlOptionsSection)
            .AddSingleton<IVCodeService, SequenceVCodeService>()
            .AddSingleton<IClock, Clock>()
            .AddSingleton(
                new AppSettings
                {
                    BaseUrl = builder.Configuration.GetBaseUrl(),
                })
            .AddTransient<IEventStore, EventStore>()
            .AddTransient<IVerenigingsRepository, VerenigingsRepository>()
            .AddTransient<IMagdaFacade, StaticMagdaFacade>()
            .AddTransient<IDuplicateVerenigingDetectionService, SearchDuplicateVerenigingDetectionService>()
            .AddMarten(postgreSqlOptionsSection, builder.Configuration)
            .AddOpenTelemetry()
            .AddHttpContextAccessor()
            .AddControllers(options => options.Filters.Add<JsonRequestFilter>());

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
            .Configure<ProblemDetailsOptions>(
                cfg =>
                {
                    foreach (var header in StartupConstants.ExposedHeaders) cfg.AllowedHeaderNames.Add(header);
                })
            .TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<ProblemDetailsOptions>, ProblemDetailsOptionsSetup>());


        builder.Services
            .AddMvcCore(
                cfg =>
                {
                    cfg.RespectBrowserAcceptHeader = false;
                    cfg.ReturnHttpNotAcceptable = true;

                    cfg.Filters.Add(new LoggingFilterFactory(StartupConstants.HttpMethodsAsString));

                    cfg.Filters.Add<OperationCancelledExceptionFilter>();

                    cfg.EnableEndpointRouting = false;
                })
            .AddCors(
                cfg =>
                {
                    cfg.AddPolicy(
                        StartupConstants.AllowAnyOrigin,
                        corsPolicy => corsPolicy
                            .AllowAnyOrigin()
                            .WithMethods(StartupConstants.HttpMethodsAsString)
                            .WithHeaders(StartupConstants.Headers)
                            .WithExposedHeaders(StartupConstants.ExposedHeaders)
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(60 * 15)));

                    cfg.AddPolicy(
                        StartupConstants.AllowSpecificOrigin,
                        corsPolicy => corsPolicy
                            .WithOrigins(builder.Configuration.GetValue<string[]>("Cors") ?? Array.Empty<string>())
                            .WithMethods(StartupConstants.HttpMethodsAsString)
                            .WithHeaders(StartupConstants.Headers)
                            .WithExposedHeaders(StartupConstants.ExposedHeaders)
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(60 * 15))
                            .AllowCredentials());
                })
            .AddControllersAsServices()
            .AddAuthorization(
                options =>
                    options.AddPolicy(
                        AdminGlobalPolicyName,
                        new AuthorizationPolicyBuilder()
                            .RequireClaim(Security.ClaimTypes.Scope, Security.Scopes.Admin)
                            .Build()))
            .AddNewtonsoftJson(
                opt =>
                {
                    opt.SerializerSettings.Converters.Add(new StringEnumConverter(new DefaultNamingStrategy(), false));
                    opt.SerializerSettings.Converters.Add(new NullOrEmptyDateOnlyJsonConvertor());
                    opt.SerializerSettings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                    opt.SerializerSettings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
            .AddXmlDataContractSerializerFormatters()
            .AddFormatterMappings()
            .AddApiExplorer();

        builder.Services.AddAuthentication(
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
                    var configOptions = builder.Configuration.GetSection(nameof(OAuth2IntrospectionOptions)).Get<OAuth2IntrospectionOptions>();
                    options.ClientId = configOptions.ClientId;
                    options.ClientSecret = configOptions.ClientSecret;
                    options.Authority = configOptions.Authority;
                    options.IntrospectionEndpoint = configOptions.IntrospectionEndpoint;
                }
            );

        builder.Services
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .AddDatabaseDeveloperPageExceptionFilter();

        var healthChecksBuilder = builder.Services.AddHealthChecks();

        var connectionStrings = builder.Configuration
            .GetSection("ConnectionStrings")
            .GetChildren();

        foreach (var connectionString in connectionStrings)
            healthChecksBuilder.AddSqlServer(
                connectionString.Value,
                name: $"sqlserver-{connectionString.Key.ToLowerInvariant()}",
                tags: new[] { StartupConstants.DatabaseTag, "sql", "sqlserver" });

        // health.AddDbContextCheck<LegacyContext>(
        //     $"dbcontext-{nameof(LegacyContext).ToLowerInvariant()}",
        //     tags: new[] { DatabaseTag, "sql", "sqlserver" });


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
                })
            .AddVersionedApiExplorer(
                cfg =>
                {
                    cfg.GroupNameFormat = "'v'VVV";
                    cfg.SubstituteApiVersionInUrl = true;
                })
            .AddApiVersioning(
                cfg =>
                {
                    cfg.ReportApiVersions = true;
                    cfg.ErrorResponses = new ProblemDetailsResponseProvider();
                })
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

        builder.Services.AddAdminApiSwagger();
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
            () => webHost.Run(),
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
                    new IPEndPoint(IPAddress.Any, port: 11004),
                    listenOptions =>
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
        jsonSerializerSettings.Converters.Add(new NullOrEmptyDateOnlyJsonConvertor());
        jsonSerializerSettings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
        jsonSerializerSettings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
        jsonSerializerSettings.NullValueHandling = NullValueHandling.Include;
        jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        JsonConvert.DefaultSettings = () => jsonSerializerSettings;
    }

    private static void ConfigureAppDomainExceptions()
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

    private static void ConfigureLifetimeHooks(WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() => Log.Information("Application started"));

        app.Lifetime.ApplicationStopping.Register(
            () =>
            {
                Log.Information("Application stopping");
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
