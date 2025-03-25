namespace AssociationRegistry.Admin.Api;

using Adapters.DuplicateVerenigingDetectionService;
using Adapters.VCodeGeneration;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Asp.Versioning.ApplicationModels;
using Notifications;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.Api.Localization;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Logging;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Be.Vlaanderen.Basisregisters.Middleware.AddProblemJsonHeader;
using Constants;
using DuplicateVerenigingDetection;
using Events;
using EventStore;
using FluentValidation;
using Formats;
using Framework;
using Grar.Clients;
using Grar.GrarUpdates.Fusies;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using Grar.GrarUpdates.Hernummering;
using Grar.GrarUpdates.LocatieFinder;
using GrarConsumer.Finders;
using GrarConsumer.Kafka;
using HostedServices.VzerMigratie;
using Hosts;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Infrastructure;
using Infrastructure.AWS;
using Infrastructure.Configuration;
using Infrastructure.ExceptionHandlers;
using Infrastructure.Extensions;
using Infrastructure.HttpClients;
using Infrastructure.Json;
using Infrastructure.Metrics;
using Infrastructure.Middleware;
using Infrastructure.ResponseWriter;
using Infrastructure.Sequence;
using JasperFx.Core;
using Kbo;
using Lamar.Microsoft.DependencyInjection;
using Magda;
using Marten;
using MessageHandling.Sqs.AddressMatch;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Oakton;
using OpenTelemetry.Extensions;
using Queries;
using Serilog;
using Serilog.Debugging;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Vereniging;
using IExceptionHandler = Be.Vlaanderen.Basisregisters.Api.Exceptions.IExceptionHandler;
using ProblemDetailsOptions = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetailsOptions;

public class Program
{
    private const string AdminGlobalPolicyName = "Admin Global";
    public const string SuperAdminPolicyName = "Super Admin";

    public static async Task Main(string[] args)
    {
        AWSConfigs.AWSRegion = RegionEndpoint.EUWest1.SystemName;

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

        ConfigureKestrel(builder);
        ConfigureWebHost(builder);
        ConfigureServices(builder);
        ConfigureHostedServices(builder);

        builder.Host.ApplyOaktonExtensions();
        builder.Host.UseLamar();

        builder.AddWolverine();

        var app = builder.Build();

        if (ProgramArguments.IsCodeGen(args))
        {
            await app.RunOaktonCommands(args);
            return;
        }

        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        await WaitFor.PostGreSQLToBecomeAvailable(logger, app.Configuration.GetPostgreSqlOptionsSection().GetConnectionString());

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
           .UseEndpoints(routeBuilder =>
            {
                routeBuilder.MapControllers().RequireAuthorization(AdminGlobalPolicyName);
            });

        ConfigureLifetimeHooks(app);

        await RunPreStartupTasks(app, logger);

        await WaitFor.ElasticSearchToBecomeAvailable(
            app.Services.GetRequiredService<ElasticClient>(),
            app.Services.GetRequiredService<ILogger<Program>>());

        await app.RunOaktonCommands(args);
    }

    private static async Task RunPreStartupTasks(WebApplication app, ILogger<Program> logger)
    {
        await ArchiveAfdelingen(app);
        await RegistreerOntbrekendeInschrijvingen(app, logger);
    }

    private static async Task RegistreerOntbrekendeInschrijvingen(WebApplication app, ILogger<Program> logger)
    {
        try
        {
            var registreerInschrijvinCatchupService = app.Services.GetRequiredService<IMagdaRegistreerInschrijvingCatchupService>();

            await registreerInschrijvinCatchupService
               .RegistreerInschrijvingVoorVerenigingenMetRechtspersoonlijkheidDieNogNietIngeschrevenZijn();
        }
        catch (Exception ex)
        {
            logger.LogWarning($"MAGDA Catchup Service: Fout bij het opstarten! ({ex.Message})");
        }
    }

    private static async Task ArchiveAfdelingen(WebApplication app)
    {
        await using var session = app.Services.GetRequiredService<IDocumentSession>();

        var queryAllRawEvents = session
                               .Events.QueryRawEventDataOnly<AfdelingWerdGeregistreerd>();

        queryAllRawEvents
           .Select(x => x.VCode)
           .ToList()
           .ForEach(key =>
            {
                app.Services.GetRequiredService<ILogger<Program>>().LogInformation(message: "Archiving {Stream}", key);
                session.Events.ArchiveStream(key);
            });

        await session.SaveChangesAsync();
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
            new IExceptionHandler[]
            {
                new BadHttpRequestExceptionHandler(problemDetailsHelper),
                new CouldNotParseRequestExceptionHandler(problemDetailsHelper),
                new UnexpectedAggregateVersionExceptionHandler(problemDetailsHelper),
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

                b.Run(
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
           .UseMiddleware<CorrelationIdMiddleware>()
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
           .UseMiddleware<UnexpectedAggregateVersionMiddleware>()
           .UseMiddleware<InitiatorHeaderMiddleware>()
           .UseMiddleware<CustomHeadersActivityMiddleware>()
           .UseResponseCompression();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var elasticSearchOptionsSection = builder.Configuration.GetElasticSearchOptionsSection();
        var postgreSqlOptionsSection = builder.Configuration.GetPostgreSqlOptionsSection();
        var magdaOptionsSection = builder.Configuration.GetMagdaOptionsSection();
        var grarOptions = builder.Configuration.GetGrarOptions();

        var magdaTemporaryVertegenwoordigersSection =
            builder.Configuration.GetMagdaTemporaryVertegenwoordigersSection(builder.Environment.IsProduction());

        var appSettings = builder.Configuration.Get<AppSettings>();

        var sqsClient = grarOptions.Sqs.UseLocalStack
            ? new AmazonSQSClient(new BasicAWSCredentials("dummy", "dummy"), new AmazonSQSConfig()
            {
                ServiceURL = "http://localhost:4566",
            })
            : new AmazonSQSClient(RegionEndpoint.EUWest1);

        builder.Services
               .AddHttpClient<AdminProjectionHostHttpClient>()
               .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(appSettings.BeheerProjectionHostBaseUrl));

        builder.Services
               .AddHttpClient<PublicProjectionHostHttpClient>()
               .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(appSettings.PublicProjectionHostBaseUrl));

        builder.Services.AddMemoryCache();

        builder.Services
               .AddHttpClient<GrarHttpClient>()
               .ConfigureHttpClient(httpClient =>
                {
                    httpClient.DefaultRequestHeaders.Add("x-api-key", grarOptions.HttpClient.ApiKey);
                    httpClient.BaseAddress = new Uri(grarOptions.HttpClient.BaseUrl);
                });

        builder.ConfigureOpenTelemetry(new Instrumentation());

        builder.Services
               .AddSingleton(postgreSqlOptionsSection)
               .AddSingleton(magdaOptionsSection)
               .AddSingleton(grarOptions)
               .AddSingleton(appSettings)
               .AddSingleton(builder.Configuration.GetSection(nameof(OAuth2IntrospectionOptions))
                                    .Get<OAuth2IntrospectionOptions>())
               .AddSingleton(magdaTemporaryVertegenwoordigersSection)
               .AddSingleton<IVCodeService, SequenceVCodeService>()
               .AddSingleton<IAmazonSQS>(sqsClient)
               .AddSingleton<IClock, Clock>()
               .AddSingleton<IGrarHttpClient>(provider => provider.GetRequiredService<GrarHttpClient>())
               .AddSingleton(new SlackWebhook(grarOptions.Kafka.SlackWebhook))
               .AddScoped(sp => new ElasticSearchOptionsService(
                              sp.GetRequiredService<ElasticSearchOptionsSection>(),
                              sp.GetRequiredService<IDocumentSession>(),
                              sp.GetRequiredService<IMemoryCache>()))
               .AddScoped(sp => new MinimumScore(sp.GetRequiredService<ElasticSearchOptionsService>().MinimumScoreDuplicateDetection))
               .AddScoped<InitiatorProvider>()
               .AddScoped<IMagdaRegistreerInschrijvingCatchupService, MagdaRegistreerInschrijvingCatchupService>()
               .AddScoped<ICorrelationIdProvider, CorrelationIdProvider>()
               .AddScoped<InitiatorProvider>()
               .AddScoped<ICommandMetadataProvider, CommandMetadataProvider>()
               .AddScoped<IMagdaGeefVerenigingService, MagdaGeefVerenigingService>()
               .AddScoped<IMagdaRegistreerInschrijvingService, MagdaRegistreerInschrijvingService>()
               .AddScoped<IMagdaClient, MagdaClient>()
               .AddScoped<TeAdresMatchenLocatieMessageHandler>()
               .AddTransient<ISqsClientWrapper, SqsClientWrapper>()
               .AddTransient<SqsClientWrapper>()
               .AddTransient<IEventStore, EventStore>()
               .AddTransient<IVerenigingsRepository, VerenigingsRepository>()
               .AddTransient<IDuplicateVerenigingDetectionService, SearchDuplicateVerenigingDetectionService>()
               .AddTransient<IGrarClient, GrarClient>()
               .AddTransient<IMagdaCallReferenceRepository, MagdaCallReferenceRepository>()
               .AddTransient<INotifier, SlackNotifier>()
               .AddTransient<ILocatieFinder, LocatieFinder>()
               .AddTransient<TeHeradresserenLocatiesMapper>()
               .AddTransient<ITeHeradresserenLocatiesProcessor, TeHeradresserenLocatiesProcessor>()
               .AddTransient<ITeOntkoppelenLocatiesProcessor, TeOntkoppelenLocatiesProcessor>()
               .AddTransient<IFusieEventProcessor, FusieEventProcessor>()
               .AddTransient<ITeHernummerenStraatEventProcessor, TeHernummerenStraatEventProcessor>()
               .AddMarten(builder.Configuration, postgreSqlOptionsSection, builder.Environment.IsDevelopment())
               .AddElasticSearch(elasticSearchOptionsSection)
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
                        foreach (var header in StartupConstants.ExposedHeaders)
                        {
                            cfg.AllowedHeaderNames.Add(header);
                        }
                    })
               .TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<ProblemDetailsOptions>, ProblemDetailsOptionsSetup>());

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
               .AddAuthorization(
                    options =>
                    {
                        options.AddPolicy(
                            AdminGlobalPolicyName,
                            new AuthorizationPolicyBuilder()
                               .RequireClaim(Security.ClaimTypes.Scope, Security.Scopes.Admin)
                               .Build());

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
                        opt.SerializerSettings.Converters.Add(
                            new StringEnumConverter(new DefaultNamingStrategy(), allowIntegerValues: false));

                        opt.SerializerSettings.Converters.Add(new NullOrEmptyDateOnlyJsonConvertor());
                        opt.SerializerSettings.Converters.Add(new NullableNullOrEmptyDateOnlyJsonConvertor());
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
                    configureOptions: options =>
                    {
                        var configOptions = builder.Configuration.GetSection(nameof(OAuth2IntrospectionOptions))
                                                   .Get<OAuth2IntrospectionOptions>();

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

        // var connectionStrings = builder.Configuration
        //                                .GetSection("ConnectionStrings")
        //                                .GetChildren();

        // foreach (var connectionString in connectionStrings)
        // {
        //     healthChecksBuilder.AddSqlServer(
        //         connectionString.Value,
        //         name: $"sqlserver-{connectionString.Key.ToLowerInvariant()}",
        //         tags: new[] { StartupConstants.DatabaseTag, "sql", "sqlserver" });
        // }

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

        builder.Services
               .AddApiVersioning(
                    cfg =>
                    {
                        cfg.ReportApiVersions = true;
                    })
               .AddApiExplorer(
                    cfg =>
                    {
                        cfg.GroupNameFormat = "'v'VVV";
                        cfg.SubstituteApiVersionInUrl = true;
                    });

        builder.Services.AddAdminApiSwagger(appSettings);
        builder.Services.AddSingleton<ProblemDetailsHelper>()
               .AddSingleton<IResponseWriter, ResponseWriter>();

        builder.Services.AddSingleton<IEventPostConflictResolutionStrategy, AddressMatchConflictResolutionStrategy>();
        builder.Services.AddSingleton<IEventPreConflictResolutionStrategy, AddressMatchConflictResolutionStrategy>();
        builder.Services.AddSingleton<EventConflictResolver>();

        builder.Services.AddSingleton<ISequenceGuarder, SequenceGuarder>();

        builder.Services
               .AddTransient<IBeheerVerenigingDetailQuery, BeheerVerenigingDetailQuery>()
               .AddTransient<IBeheerVerenigingenZoekQuery, BeheerVerenigingenZoekQuery>()
               .AddTransient<IGetNamesForVCodesQuery, GetNamesForVCodesQuery>();
    }

    private static void ConfigureHostedServices(WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<VzerMigratieService>();

        ConfigureAddresskafkaConsumer(builder);
    }

    private static void ConfigureAddresskafkaConsumer(WebApplicationBuilder builder)
    {
        var grarOptions = builder.Configuration.GetGrarOptions();

        builder.Services
               .AddSingleton(new AddressKafkaConfiguration(grarOptions.Kafka));

        if (!grarOptions.Kafka.Enabled)
            return;

        builder.Services.AddHostedService<AddressKafkaConsumer>();
    }

    private static void ConfigureWebHost(WebApplicationBuilder builder)
        => builder.WebHost.CaptureStartupErrors(captureStartupErrors: true);

    private static void ConfigureKestrel(WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(
            options =>
            {
                options.AddServerHeader = false;

                options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(value: 120);

                options.Listen(
                    new IPEndPoint(IPAddress.Any, port: 11004),
                    configure: listenOptions =>
                    {
                        listenOptions.UseConnectionLogging();
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    });

                options.ConfigureEndpointDefaults(c => c.Protocols = HttpProtocols.Http2);
            });
    }

    private static void ConfigureEncoding()
        => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    private static void ConfigureJsonSerializerSettings()
    {
        var jsonSerializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();
        jsonSerializerSettings.Converters.Add(new NullOrEmptyDateOnlyJsonConvertor());
        jsonSerializerSettings.Converters.Add(new NullableNullOrEmptyDateOnlyJsonConvertor());
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
                messageTemplate: "FirstChanceException event raised in {AppDomain}",
                AppDomain.CurrentDomain.FriendlyName);

        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
            Log.Fatal(
                (Exception)eventArgs.ExceptionObject,
                messageTemplate: "Encountered a fatal exception, exiting program");
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

public class NullNotifier : INotifier
{
    public Task Notify(IMessage message) => Task.CompletedTask;
}
