namespace AssociationRegistry.Admin.Api;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Reflection;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.Api.Localization;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Logging;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger;
using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger.ReDoc;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Be.Vlaanderen.Basisregisters.DataDog.Tracing;
using Be.Vlaanderen.Basisregisters.DataDog.Tracing.AspNetCore;
using Be.Vlaanderen.Basisregisters.Middleware.AddProblemJsonHeader;
using ConfigurationBindings;
using Constants;
using Events;
using Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Framework;
using Infrastructure.Configuration;
using Infrastructure.Json;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
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
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OpenTelemetry.Extensions;
using Serilog;
using VCodes;
using Vereniging;
using Verenigingen;
using Verenigingen.VCodes;

/// <summary>Represents the startup process for the application.</summary>
public class Startup
{
    private static string[] HttpMethodsAsString
        => new[]
        {
            HttpMethod.Get,
            HttpMethod.Head,
            HttpMethod.Post,
            HttpMethod.Put,
            HttpMethod.Patch,
            HttpMethod.Delete,
            HttpMethod.Options,
        }.Select(m => m.Method).ToArray();

    private static readonly string[] Headers =
    {
        HeaderNames.Accept,
        HeaderNames.ContentType,
        HeaderNames.Origin,
        HeaderNames.Authorization,
        HeaderNames.IfMatch,
        // ExtractFilteringRequestExtension.HeaderName,
        // AddSortingExtension.HeaderName,
        // AddPaginationExtension.HeaderName,
    };

    private static readonly string[] ExposedHeaders =
    {
        HeaderNames.Location,
        // ExtractFilteringRequestExtension.HeaderName,
        // AddSortingExtension.HeaderName,
        // AddPaginationExtension.HeaderName,
        AddVersionHeaderMiddleware.HeaderName,
        AddCorrelationIdToResponseMiddleware.HeaderName,
        AddHttpSecurityHeadersMiddleware.PoweredByHeaderName,
        AddHttpSecurityHeadersMiddleware.ContentTypeOptionsHeaderName,
        AddHttpSecurityHeadersMiddleware.FrameOptionsHeaderName,
        AddHttpSecurityHeadersMiddleware.XssProtectionHeaderName,
    };

    private const string DatabaseTag = "db";
    private const string Culture = "en-GB";

    private const string AllowAnyOrigin = "AllowAnyOrigin";
    private const string AllowSpecificOrigin = "AllowSpecificOrigin";

    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>Configures services for the application.</summary>
    /// <param name="services">The collection of services to configure the application with.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        var postgreSqlOptionsSection = _configuration.GetSection(PostgreSqlOptionsSection.Name)
            .Get<PostgreSqlOptionsSection>();

        ThrowIfInvalidPostgreSqlOptions(postgreSqlOptionsSection);

        AddSwagger(services);

        services.AddMarten(postgreSqlOptionsSection, _configuration);

        services.AddMediatR(typeof(CommandEnvelope<>));
        services.AddTransient<IVerenigingsRepository, VerenigingsRepository>();
        services.AddTransient<IEventStore, EventStore>();
        services.AddSingleton<IVCodeService, SequenceVCodeService>();
        services.AddSingleton<IClock, Clock>();
        services.AddSingleton(
            new AppSettings
            {
                BaseUrl = GetBaseUrl(_configuration),
            });

        services.AddOpenTelemetry();

        services.TryAddEnumerable(ServiceDescriptor.Transient<IApiControllerSpecification, ApiControllerSpec>());

        services
            .AddHttpContextAccessor()
            ;

        AddConfigureProblemDetails(services);


        services
            .AddMvcCore(
                cfg =>
                {
                    cfg.RespectBrowserAcceptHeader = false;
                    cfg.ReturnHttpNotAcceptable = true;

                    cfg.Filters.Add(new LoggingFilterFactory(HttpMethodsAsString));

                    cfg.Filters.Add<OperationCancelledExceptionFilter>();

                    cfg.Filters.Add(new DataDogTracingFilter());

                    cfg.EnableEndpointRouting = false;
                })
            .AddCors(
                cfg =>
                {
                    cfg.AddPolicy(
                        AllowAnyOrigin,
                        corsPolicy => corsPolicy
                            .AllowAnyOrigin()
                            .WithMethods(HttpMethodsAsString)
                            .WithHeaders(Headers)
                            .WithExposedHeaders(ExposedHeaders)
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(60 * 15)));

                    cfg.AddPolicy(
                        AllowSpecificOrigin,
                        corsPolicy => corsPolicy
                            .WithOrigins(_configuration.GetSection("Cors").Get<string[]>())
                            .WithMethods(HttpMethodsAsString)
                            .WithHeaders(Headers)
                            .WithExposedHeaders(ExposedHeaders)
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

        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddDatabaseDeveloperPageExceptionFilter();

        var healthChecksBuilder = services.AddHealthChecks();

        var connectionStrings = _configuration
            .GetSection("ConnectionStrings")
            .GetChildren();

        foreach (var connectionString in connectionStrings)
            healthChecksBuilder.AddSqlServer(
                connectionString.Value,
                name: $"sqlserver-{connectionString.Key.ToLowerInvariant()}",
                tags: new[] { DatabaseTag, "sql", "sqlserver" });

        // health.AddDbContextCheck<LegacyContext>(
        //     $"dbcontext-{nameof(LegacyContext).ToLowerInvariant()}",
        //     tags: new[] { DatabaseTag, "sql", "sqlserver" });


        services
            .AddLocalization(cfg => cfg.ResourcesPath = "Resources")
            .AddSingleton<IStringLocalizerFactory, SharedStringLocalizerFactory<StartupDefaults.DefaultResources>>()
            .AddSingleton<ResourceManagerStringLocalizerFactory, ResourceManagerStringLocalizerFactory>()
            .Configure<RequestLocalizationOptions>(
                opts =>
                {
                    opts.DefaultRequestCulture = new RequestCulture(new CultureInfo(Culture));
                    opts.SupportedCultures = new[] { new CultureInfo(Culture) };
                    opts.SupportedUICultures = new[] { new CultureInfo(Culture) };

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
            .AddSwagger<Startup>(
                new SwaggerOptions
                {
                    ApiInfoFunc = (_, description) => new OpenApiInfo
                    {
                        Version = description.ApiVersion.ToString(),
                        Title = "Basisregisters Vlaanderen Verenigingsregister Beheer API",
                        Description = GetApiLeadingText(description),
                        Contact = new OpenApiContact
                        {
                            Name = "Digitaal Vlaanderen",
                            Email = "digitaal.vlaanderen@vlaanderen.be",
                            Url = new Uri("https://beheer.verenigingen.vlaanderen.be"),
                        },
                    },
                    XmlCommentPaths = new[] { typeof(Startup).GetTypeInfo().Assembly.GetName().Name! },
                    AdditionalHeaderOperationFilters = new List<HeaderOperationFilter>(),
                    Servers = new List<Server>(),
                    CustomSortFunc = SortByTag.Sort,
                })
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

        ValidatorOptions.Global.DisplayNameResolver =
            (_, member, _) =>
                member != null
                    ? GlobalStringLocalizer.Instance.GetLocalizer<StartupDefaults.DefaultResources>().GetString(() => member.Name)
                    : null;

        services.AddSingleton<ProblemDetailsHelper>();
    }

    private static void AddConfigureProblemDetails(IServiceCollection services)
    {
        services.ConfigureOptions<ProblemDetailsSetup>();
        services.Configure<ProblemDetailsOptions>(
            cfg =>
            {
                foreach (var header in ExposedHeaders)
                {
                    if (!cfg.AllowedHeaderNames.Contains(header))
                        cfg.AllowedHeaderNames.Add(header);
                }
            });

        services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<ProblemDetailsOptions>, ProblemDetailsOptionsSetup>());
    }

    private static string GetBaseUrl(IConfiguration configuration)
        => TrimTrailingSlash(configuration.GetValue<string>("BaseUrl"));

    private static string TrimTrailingSlash(string baseUrl)
        => baseUrl.TrimEnd('/');

    private static void AddSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(
            options =>
            {
                options.SupportNonNullableReferenceTypes();
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
    )
    {
        GlobalStringLocalizer.Instance = new GlobalStringLocalizer(app.ApplicationServices.GetRequiredService<IServiceProvider>());

        if (env.IsDevelopment())
            app
                .UseDeveloperExceptionPage()
                .UseMigrationsEndPoint()
                .UseBrowserLink();

        app.UseCors(policyName: AllowSpecificOrigin);

        app.UseMiddleware<ProblemDetailsMiddleware>();
        var problemDetailsHelper = app.ApplicationServices.GetRequiredService<ProblemDetailsHelper>();
        var logger = loggerFactory.CreateLogger<ApiExceptionHandler>();
        var exceptionHandler = new ExceptionHandler(logger, Array.Empty<ApiProblemDetailsExceptionMapping>(), Array.Empty<IExceptionHandler>(), problemDetailsHelper);

        app.UseExceptionHandler404Allowed(
            builder =>
            {
                builder.UseCors(policyName: AllowSpecificOrigin);

                builder.UseMiddleware<ProblemDetailsMiddleware>();
                ConfigureMiddleware(builder);


                var requestLocalizationOptions1 = serviceProvider
                    .GetRequiredService<IOptions<RequestLocalizationOptions>>()
                    .Value;

                builder.UseRequestLocalization(requestLocalizationOptions1);

                builder
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

        var requestLocalizationOptions = serviceProvider
            .GetRequiredService<IOptions<RequestLocalizationOptions>>()
            .Value;

        app.UseRequestLocalization(requestLocalizationOptions);

        app.UseSwaggerDocumentation(
            new SwaggerDocumentationOptions
            {
                ApiVersionDescriptionProvider = apiVersionProvider,
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

        var traceAgent = serviceProvider.GetRequiredService<TraceAgent>();
        appLifetime.ApplicationStarted.Register(() => Log.Information("Application started."));

        appLifetime.ApplicationStopping.Register(
            () =>
            {
                traceAgent.OnCompleted();
                traceAgent.Completion.Wait();

                Log.Information("Application stopping.");
                Log.CloseAndFlush();
            });

        Console.CancelKeyPress += (_, eventArgs) =>
        {
            appLifetime.StopApplication();

            // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
            eventArgs.Cancel = true;
        };
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

    private static string GetApiLeadingText(ApiVersionDescription description)
        => $"Momenteel leest u de documentatie voor versie {description.ApiVersion} van de Basisregisters Vlaanderen Verenigingsregister Beheer API{string.Format(description.IsDeprecated ? ", **deze API versie is niet meer ondersteund * *." : ".")}";

    private static void ThrowIfInvalidPostgreSqlOptions(PostgreSqlOptionsSection postgreSqlOptions)
    {
        const string sectionName = nameof(PostgreSqlOptionsSection);
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(postgreSqlOptions.Database, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Database)}");
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(postgreSqlOptions.Host, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Host)}");
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(postgreSqlOptions.Username, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Username)}");
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(postgreSqlOptions.Password, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Password)}");
    }
}
