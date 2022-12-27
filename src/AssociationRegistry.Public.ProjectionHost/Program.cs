using AssociationRegistry.OpenTelemetry.Extensions;
using AssociationRegistry.Public.ProjectionHost;
using AssociationRegistry.Public.ProjectionHost.ConfigurationBindings;
using AssociationRegistry.Public.ProjectionHost.Constants;
using AssociationRegistry.Public.ProjectionHost.Extensions;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Json;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Marten.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

builder.Services.RegisterDomainEventHandlers(typeof(Program).Assembly);

builder.Services
    .AddTransient<IElasticRepository, ElasticRepository>()
    .AddSingleton<IVerenigingBrolFeeder, VerenigingBrolFeeder>();

var martenConfiguration = builder.Services.AddMarten(
    serviceProvider =>
    {
        var postgreSqlOptions = builder.Configuration.GetSection(PostgreSqlOptionsSection.Name)
            .Get<PostgreSqlOptionsSection>();
        var connectionString = GetPostgresConnectionString(postgreSqlOptions);

        var opts = new StoreOptions();

        opts.Connection(connectionString);

        opts.Events.StreamIdentity = StreamIdentity.AsString;

        opts.Events.MetadataConfig.EnableAll();

        opts.Projections.Add<VerenigingDetailProjection>();
        opts.Projections.Add(
            new MartenSubscription(
                new MartenEventsConsumer(builder.Services.BuildServiceProvider())),
            ProjectionLifecycle.Async);

        opts.Serializer(CreateCustomMartenSerializer());
        return opts;
    });

if (builder.Configuration["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
        martenConfiguration.AddAsyncDaemon(DaemonMode.Solo);

var postgreSqlOptions = builder.Configuration.GetSection(PostgreSqlOptionsSection.Name)
    .Get<PostgreSqlOptionsSection>();

ConfigHelpers.ThrowIfInvalidPostgreSqlOptions(postgreSqlOptions);

var elasticSearchOptions = builder.Configuration.GetSection("ElasticClientOptions")
    .Get<ElasticSearchOptionsSection>();

ConfigHelpers.ThrowIfInvalidElasticOptions(elasticSearchOptions);

builder.Services.AddOpenTelemetry();
builder.Services.AddElasticSearch(elasticSearchOptions);
builder.Services.AddMvc();

AddSwagger(builder.Services);


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

await DistributedLock<Program>.RunAsync(
    async () =>
    {
        app.Run();
    },
    DistributedLockOptions.LoadFromConfiguration(builder.Configuration),
    NullLogger.Instance);

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

static string GetPostgresConnectionString(PostgreSqlOptionsSection postgreSqlOptions)
    => $"host={postgreSqlOptions.Host};" +
       $"database={postgreSqlOptions.Database};" +
       $"password={postgreSqlOptions.Password};" +
       $"username={postgreSqlOptions.Username}";

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
