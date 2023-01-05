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
using Constants;
using Events;
using Extensions;
using Infrastructure.Configuration;
using Infrastructure.Json;
using Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.VCodes;
using Framework;
using AssociationRegistry.OpenTelemetry.Extensions;
using VCodes;
using Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.Api.Localization;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Logging;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger;
using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger.ReDoc;
using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Be.Vlaanderen.Basisregisters.Middleware.AddProblemJsonHeader;
using Destructurama;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Debugging;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = Directory.GetCurrentDirectory(),
                WebRootPath = "wwwroot",
            });

        builder.Configuration
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .AddInMemoryCollection();

        SelfLog.Enable(Console.WriteLine);

        ConfigureEncoding();
        ConfigureJsonSerializerSettings();
        ConfigureAppDomainExceptions();
        ConfigereKesrel(builder);
        ConfigureLogger(builder);
        ConfigureWebHost(builder);
        ConfigureServices(builder);

        var app = builder.Build();

        GlobalStringLocalizer.Instance = new GlobalStringLocalizer(app.Services);

        if (app.Environment.IsDevelopment())
            app
                .UseDeveloperExceptionPage()
                .UseMigrationsEndPoint()
                .UseBrowserLink();

        app.UseCors(policyName: StartupConstants.AllowSpecificOrigin);

        var problemDetailsHelper = app.Services.GetRequiredService<ProblemDetailsHelper>();
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger<ApiExceptionHandler>();
        var exceptionHandler = new ExceptionHandler(
            logger,
            Array.Empty<ApiProblemDetailsExceptionMapping>(),
            Array.Empty<IExceptionHandler>(),
            problemDetailsHelper);

        // Deze volgorde is belangrijk ! DKW
        app.UseMiddleware<ProblemDetailsMiddleware>();
        app.UseExceptionHandler404Allowed(
            b =>
            {
                b.UseCors(policyName: StartupConstants.AllowSpecificOrigin);

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

        ConfigureMiddleware(app);

        app.UseMiddleware<AddProblemJsonHeaderMiddleware>();

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

        var requestLocalizationOptions = app.Services
            .GetRequiredService<IOptions<RequestLocalizationOptions>>()
            .Value;

        app.UseRequestLocalization(requestLocalizationOptions);

        app.UseSwaggerDocumentation(
            new SwaggerDocumentationOptions
            {
                ApiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>(),
                DocumentTitleFunc = groupName => $"Basisregisters Vlaanderen - Verenigingsregister Beheer API {groupName}",
                CSharpClient =
                {
                    ClassName = "Verenigingsregister",
                    Namespace = "Be.Vlaanderen.Basisregisters",
                },
                TypeScriptClient =
                {
                    ClassName = "Verenigingsregister",
                },
            });

        app.UseMvc();

        app.Lifetime.ApplicationStarted.Register(() => Log.Information("Application started."));

        app.Lifetime.ApplicationStopping.Register(
            () =>
            {
                Log.Information("Application stopping.");
                Log.CloseAndFlush();
            });

        Console.CancelKeyPress += (_, eventArgs) =>
        {
            app.Lifetime.StopApplication();

            // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
            eventArgs.Cancel = true;
        };

        app.Run();


        static void ConfigureMiddleware(IApplicationBuilder app)
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

        static void ConfigureServices(WebApplicationBuilder builder)
        {
            var postgreSqlOptionsSection = builder.Configuration.GetPostgreSqlOptionsSection();

            builder.Services
                .AddSingleton<IVCodeService, SequenceVCodeService>()
                .AddSingleton<IClock, Clock>()
                .AddSingleton(
                    new AppSettings
                    {
                        BaseUrl = builder.Configuration.GetBaseUrl(),
                    })
                .AddTransient<IEventStore, EventStore>()
                .AddTransient<IVerenigingsRepository, VerenigingsRepository>()
                .AddMarten(postgreSqlOptionsSection, builder.Configuration)
                .AddMediatR(typeof(CommandEnvelope<>))
                .AddOpenTelemetry()
                .AddHttpContextAccessor();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IApiControllerSpecification, ApiControllerSpec>());

            builder.Services
                .AddSingleton(
                    new StartupConfigureOptions()
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
                            if (!cfg.AllowedHeaderNames.Contains(header))
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
                .AddAuthorization()
                .AddNewtonsoftJson(
                    opt =>
                    {
                        opt.SerializerSettings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                        opt.SerializerSettings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
                        opt.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                        opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    })
                .AddXmlDataContractSerializerFormatters()
                .AddFormatterMappings()
                .AddApiExplorer();

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

            builder.Services
                .AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly())
                .AddSwaggerGen(
                    options =>
                    {
                        AddXmlComments(options, Assembly.GetExecutingAssembly().GetName().Name!, Assembly.GetExecutingAssembly());
                        options.DescribeAllParametersInCamelCase();
                        options.SupportNonNullableReferenceTypes();
                        options.MapType<DateOnly>(
                            () => new OpenApiSchema
                            {
                                Type = "string",
                                Format = "date",
                                Pattern = "yyyy-MM-dd",
                            });
                        options.CustomSchemaIds(type => type.FullName);
                        options.SwaggerDoc(
                            "v1",
                            new OpenApiInfo
                            {
                                Version = "v1",
                                Title = "Basisregisters Vlaanderen Verenigingsregister Beheer API",
                                Description = "Momenteel leest u de documentatie voor versie v1 van de Basisregisters Vlaanderen Verenigingsregister Beheer API.",
                                Contact = new OpenApiContact
                                {
                                    Name = "Digitaal Vlaanderen",
                                    Email = "digitaal.vlaanderen@vlaanderen.be",
                                    Url = new Uri("https://beheer.verenigingen.vlaanderen.be"),
                                },
                            });
                        // Apply [SwaggerRequestExample] & [SwaggerResponseExample]
                        options.ExampleFilters();

                        // Add an AutoRest vendor extension (see https://github.com/Azure/autorest/blob/master/docs/extensions/readme.md#x-ms-enum) to inform the AutoRest tool how enums should be modelled when it generates the API client.
                        options.SchemaFilter<AutoRestSchemaFilter>();

                        // Fix up some Swagger parameter values by discovering them from ModelMetadata and RouteInfo
                        options.OperationFilter<SwaggerDefaultValues>();

                        // Apply [Description] on Response properties
                        options.OperationFilter<DescriptionOperationFilter>();

                        // Adds an Upload button to endpoints which have [AddSwaggerFileUploadButton]
                        //x.OperationFilter<AddFileParamTypesOperationFilter>(); Marked AddFileParamTypesOperationFilter as Obsolete, because Swashbuckle 4.0 supports IFormFile directly.

                        // Apply [SwaggerResponseHeader] to headers
                        options.OperationFilter<AddResponseHeadersFilter>();

                        // Apply [ApiExplorerSettings(GroupName=...)] property to tags.
                        options.OperationFilter<TagByApiExplorerSettingsOperationFilter>();

                        // Adds a 401 Unauthorized and 403 Forbidden response to every action which requires authorization
                        options.OperationFilter<AuthorizationResponseOperationFilter>();

                        // Adds "(Auth)" to the summary so that you can see which endpoints have Authorization
                        options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                        options.OrderActionsBy(SortByTag.Sort);

                        options.DocInclusionPredicate((_, _) => true);
                    })
                .AddSwaggerGenNewtonsoftSupport();

            ValidatorOptions.Global.DisplayNameResolver =
                (_, member, _) =>
                    member != null
                        ? GlobalStringLocalizer.Instance.GetLocalizer<StartupDefaults.DefaultResources>().GetString(() => member.Name)
                        : null;

            builder.Services.AddSingleton<ProblemDetailsHelper>();
        }

        static void ConfigureWebHost(WebApplicationBuilder builder)
        {
            builder.WebHost.CaptureStartupErrors(true);
        }

        static void ConfigureLogger(WebApplicationBuilder builder)
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

        static void RunWithLock<T>(IWebHostBuilder webHostBuilder) where T : class
        {
            var webHost = webHostBuilder.Build();
            var services = webHost.Services;
            var logger = services.GetRequiredService<ILogger<T>>();

            DistributedLock<T>.Run(
                () => webHost.Run(),
                DistributedLockOptions.Defaults,
                logger);
        }

        static void ConfigereKesrel(WebApplicationBuilder builder)
        {
            builder.WebHost.ConfigureKestrel(
                options =>
                {
                    options.AddServerHeader = false;

                    options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(120);

                    options.Listen(
                        new IPEndPoint(IPAddress.Any, 11004),
                        listenOptions =>
                        {
                            listenOptions.UseConnectionLogging();
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });
                });
        }

        static void ConfigureEncoding()
            => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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

        static void AddXmlComments(SwaggerGenOptions swaggerGenOptions, string name, Assembly assembly)
        {
            var possiblePaths = new[]
            {
                CreateXmlCommentsPath(AppContext.BaseDirectory, name),
                CreateXmlCommentsPath(Directory.GetParent(assembly.Location)!.FullName, name),
                CreateXmlCommentsPath(Path.GetDirectoryName(assembly.Location)!, name),
            };

            foreach (var possiblePath in possiblePaths)
            {
                if (!File.Exists(possiblePath))
                    continue;

                swaggerGenOptions.IncludeXmlComments(possiblePath);
                return;
            }

            throw new ApplicationException(
                $"Could not find swagger xml docs. Locations where I searched:\n\t- {string.Join("\n\t-", possiblePaths)}");
        }

        static string CreateXmlCommentsPath(string directory, string name)
            => Path.Combine(directory, $"{name}.xml");
    }
}
