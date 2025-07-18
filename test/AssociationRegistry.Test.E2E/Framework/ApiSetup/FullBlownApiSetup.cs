namespace AssociationRegistry.Test.E2E.Framework.ApiSetup;

using Admin.Api;
using Admin.Api.Infrastructure.Extensions;
using Admin.ProjectionHost.Projections;
using Alba;
using AlbaHost;
using Amazon.SQS;
using AssociationRegistry.Framework;
using Common.Clients;
using Common.Framework;
using Grar.NutsLau;
using Hosts.Configuration;
using IdentityModel.AspNetCore.OAuth2Introspection;
using JasperFx.CommandLine;
using JasperFx.Events.Daemon;
using Marten;
using Marten.Events.Daemon;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;
using NodaTime;
using NodaTime.Text;
using Oakton;
using Scenarios.Givens.FeitelijkeVereniging;
using System.Diagnostics;
using TestClasses;
using Vereniging;
using Xunit;
using ProjectionHostProgram = Public.ProjectionHost.Program;

public class FullBlownApiSetup : IAsyncLifetime, IApiSetup, IDisposable
{
    public FullBlownApiSetup()
    {
    }

    public string? AuthCookie { get; private set; }
    public ILogger<Program> Logger { get; private set; }
    public IAlbaHost AdminApiHost { get; private set; }
    public IAlbaHost AcmApiHost { get; private set; }
    public IAlbaHost AdminProjectionHost { get; private set; }
    public IAlbaHost PublicProjectionHost { get; private set; }
    public IAlbaHost PublicApiHost { get; private set; }

    public async ValueTask InitializeAsync()
    {
        SetUpAdminApiConfiguration();

        JasperFxEnvironment.AutoStartHost = true;

        var adminApiHost = await AlbaHost.For<Program>(ConfigureForTesting("adminapi"));

        var clients = new Clients(adminApiHost.Services.GetRequiredService<OAuth2IntrospectionOptions>(),
                                  createClientFunc: () => new HttpClient());


        SuperAdminHttpClient = clients.SuperAdmin.HttpClient;
        UnautenticatedClient = clients.Unauthenticated.HttpClient;
        UnauthorizedClient = clients.Unauthorized.HttpClient;

        AdminApiHost = adminApiHost.EnsureEachCallIsAuthenticated(clients.Authenticated.HttpClient);
        AdminHttpClient = clients.Authenticated.HttpClient;

        await AdminApiHost.ResetAllMartenDataAsync();

        var elasticSearchOptions = AdminApiHost.Server.Services.GetRequiredService<IConfiguration>().GetElasticSearchOptionsSection();
        ElasticClient = ElasticSearchExtensions.CreateElasticClient(elasticSearchOptions, NullLogger.Instance);
        ElasticClient.Indices.DeleteAsync(elasticSearchOptions.Indices.DuplicateDetection).GetAwaiter().GetResult();

        await InsertNutsLauInfo();

        AdminProjectionHost = await AlbaHost.For<Admin.ProjectionHost.Program>(
            ConfigureForTesting("adminproj"));

        Logger = AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

        PublicProjectionHost = await AlbaHost.For<ProjectionHostProgram>(
            ConfigureForTesting("publicproj"));

        PublicApiHost = await AlbaHost.For<Public.Api.Program>(
            ConfigureForTesting("publicapi"));

        AcmApiHost = (await AlbaHost.For<Acm.Api.Program>(
                ConfigureForTesting("acmapi")))
           .EnsureEachCallIsAuthenticatedForAcmApi();

        using var scope = AdminApiHost.Services.CreateScope();
        _serviceProvider = scope.ServiceProvider;

        SqsClientWrapper = _serviceProvider.GetRequiredService<ISqsClientWrapper>();
        AmazonSqs = _serviceProvider.GetRequiredService<IAmazonSQS>();
        VCodeService = _serviceProvider.GetRequiredService<IVCodeService>();

        ElasticClient = _serviceProvider.GetRequiredService<IElasticClient>();
        await AdminApiHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        await using var session = PublicApiHost.DocumentStore().LightweightSession();

        // await ExecuteGiven(new RandomEventSequenceScenario());
         // await ExecuteGiven(new MassiveRandomEventSequenceScenario());

        await session.SaveChangesAsync();

        await AdminProjectionHost.StartAsync();
        await PublicProjectionHost.StartAsync();

    }

    public IProjectionDaemon AcmProjectionDaemon { get; set; }
    public IProjectionDaemon PublicProjectionDaemon { get; set; }
    public IVCodeService VCodeService { get; set; }

    private async Task InsertNutsLauInfo()
    {
        var documentStore = AdminApiHost.DocumentStore();

        await using var session = documentStore.LightweightSession();

        session.StoreObjects(NutsLauInfoMock.All);
        await session.SaveChangesAsync();
    }

    public IProjectionDaemon AdminProjectionDaemon { get; private set; }
    public IElasticClient ElasticClient { get; set; }
    public HttpClient SuperAdminHttpClient { get; private set; }
    public HttpClient UnautenticatedClient { get; private set; }
    public HttpClient UnauthorizedClient { get; private set; }
    public HttpClient AdminHttpClient { get; private set; }

    private void SetUpAdminApiConfiguration()
    {
        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.development.json", false)
                           .AddJsonFile("appsettings.e2e.json", false)
                           .AddJsonFile($"appsettings.e2e.adminapi.json", false)
                           .Build();

        AdminApiConfiguration = configuration;
    }

    public IAmazonSQS AmazonSqs { get; set; }
    public ISqsClientWrapper SqsClientWrapper { get; set; }

    private Action<IWebHostBuilder> ConfigureForTesting(string name)
    {
        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.development.json", false)
                           .AddJsonFile("appsettings.e2e.json", false)
                           .AddJsonFile($"appsettings.e2e.{name}.json", false)
                           .Build();

        return b =>
        {
            b.UseEnvironment("Development");
            b.UseContentRoot(Directory.GetCurrentDirectory());

            b.UseConfiguration(configuration);

            b.ConfigureServices((context, services) =>
              {
                  context.HostingEnvironment.EnvironmentName = "Development";
              })
             .UseSetting(key: "ASPNETCORE_ENVIRONMENT", value: "Development")
             .UseSetting(key: "ApplyAllDatabaseChangesDisabled", value: "true");
        };
    }

    public IConfigurationRoot AdminApiConfiguration { get; set; }

    public async ValueTask DisposeAsync()
    {
        await AdminApiHost.StopAsync();
        await PublicApiHost.StopAsync();
        await AdminProjectionHost.StopAsync();
        await PublicProjectionHost.StopAsync();
        await AcmApiHost.StopAsync();
        await AdminApiHost.DisposeAsync();
        await PublicApiHost.DisposeAsync();
        await PublicProjectionHost.DisposeAsync();
        await AdminProjectionHost.DisposeAsync();
        await AcmApiHost.DisposeAsync();
    }

    public async Task<Dictionary<string, JasperFx.Events.IEvent[]>> ExecuteGiven(IScenario scenario)
    {
        var documentStore = AdminApiHost.DocumentStore();

        await using var session = documentStore.LightweightSession();
        session.SetHeader(MetadataHeaderNames.Initiator, value: "metadata.Initiator");
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(new Instant()));
        session.CorrelationId = Guid.NewGuid().ToString();

        var givenEvents = await scenario.GivenEvents(AdminApiHost.Services.GetRequiredService<IVCodeService>());

        var executedEvents = new Dictionary<string, JasperFx.Events.IEvent[]>();
        if (givenEvents.Length == 0)
            return [];

        foreach (var eventsPerStream in givenEvents)
        {
            var streamAction = session.Events.Append(eventsPerStream.Key, eventsPerStream.Value);
            if(!executedEvents.ContainsKey(streamAction.Key))
            {
                executedEvents.Add(streamAction.Key, streamAction.Events.ToArray());
            }
            else
            {
                executedEvents[streamAction.Key] = streamAction.Events.Concat(executedEvents[streamAction.Key]).ToArray();
            }
        }

        await session.SaveChangesAsync();

        return executedEvents;
    }

    public async Task RefreshIndices()
        => await ElasticClient.Indices.RefreshAsync(Indices.AllIndices);

    private readonly Dictionary<string, object> _ranContexts = new();
    private IServiceProvider _serviceProvider;

    public void Dispose()
    {
        var tasks = new Task[2]{Task.Run(async ()
                     =>
                 {
                     var sw = new Stopwatch();
                     sw.Start();

                     if (AdminProjectionDaemon != null)
                         await AdminProjectionDaemon.RebuildProjectionAsync(ProjectionNames.BeheerZoekV2, CancellationToken.None);

                     sw.Stop();
                 }),

            Task.Run(async ()
                         =>
                     {
                         var sw = new Stopwatch();
                         sw.Start();

                         if (AdminProjectionDaemon != null)
                             await AdminProjectionDaemon.RebuildProjectionAsync(ProjectionNames.BeheerZoek, CancellationToken.None);

                         sw.Stop();
                     })
        };

        Task.WhenAll(tasks).ConfigureAwait(false).GetAwaiter().GetResult();


        AdminApiHost.Dispose();
        AcmApiHost.Dispose();
        AdminProjectionHost.Dispose();
        PublicProjectionHost.Dispose();
        PublicApiHost.Dispose();
        AdminProjectionDaemon.Dispose();
        SuperAdminHttpClient.Dispose();
        UnautenticatedClient.Dispose();
        UnauthorizedClient.Dispose();
        AdminHttpClient.Dispose();
        AmazonSqs.Dispose();
    }
}
