namespace AssociationRegistry.Test.E2E.Framework.ApiSetup;

using Admin.Api;
using Admin.Api.Infrastructure.Extensions;
using Alba;
using AlbaHost;
using Amazon.SQS;
using AssociationRegistry.Framework;
using Common.Clients;
using Common.Framework;
using Grar.NutsLau;
using Hosts.Configuration;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Marten;
using Marten.Events;
using Marten.Events.Daemon;
using Marten.Events.Daemon.Coordination;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;
using NodaTime;
using NodaTime.Text;
using Oakton;
using Scenarios.Requests;
using TestClasses;
using Vereniging;
using Xunit;
using ProjectionHostProgram = Public.ProjectionHost.Program;

public class FullBlownApiSetup : IAsyncLifetime, IApiSetup
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

    public async Task InitializeAsync()
    {
        SetUpAdminApiConfiguration();

        OaktonEnvironment.AutoStartHost = true;

        var adminApiHost = await AlbaHost.For<Program>(ConfigureForTesting("adminapi"));

        var clients = new Clients(adminApiHost.Services.GetRequiredService<OAuth2IntrospectionOptions>(),
                                  createClientFunc: () => new HttpClient());

        var elasticSearchOptions = AdminApiConfiguration.GetElasticSearchOptionsSection();
        ElasticClient = ElasticSearchExtensions.CreateElasticClient(elasticSearchOptions, NullLogger.Instance);
        ElasticClient.Indices.DeleteAsync(elasticSearchOptions.Indices.DuplicateDetection).GetAwaiter().GetResult();

        SuperAdminHttpClient = clients.SuperAdmin.HttpClient;
        UnautenticatedClient = clients.Unauthenticated.HttpClient;
        UnauthorizedClient = clients.Unauthorized.HttpClient;

        AdminApiHost = adminApiHost.EnsureEachCallIsAuthenticated(clients.Authenticated.HttpClient);
        AdminHttpClient = clients.Authenticated.HttpClient;

        await AdminApiHost.ResetAllMartenDataAsync();

        InsertWerkingsgebieden();

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

        SqsClientWrapper = AdminApiHost.Services.GetRequiredService<ISqsClientWrapper>();
        AmazonSqs = AdminApiHost.Services.GetRequiredService<IAmazonSQS>();

        ElasticClient = AdminApiHost.Services.GetRequiredService<IElasticClient>();
        await AdminApiHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        AdminProjectionDaemon = AdminProjectionHost.Services.GetRequiredService<IProjectionCoordinator>().DaemonForMainDatabase();

        var agents = AdminProjectionDaemon.CurrentAgents().Select(x => new
        {
            x.Name.Identity, x.Position, x.Status
        });


        while (!agents.Any())
        {
            Logger.LogInformation("Daemon Startup {@Says}", agents.Select(x => $" {x.Identity}: {x.Position} ({x.Status})|"));
            await Task.Delay(500);
            agents = AdminProjectionDaemon.CurrentAgents().Select(x => new
            {
                x.Name.Identity, x.Position, x.Status
            });

        }
        Logger.LogInformation("Daemon Startup {@Says}", agents.Select(x => $" {x.Identity}: {x.Position} ({x.Status})|"));
    }

    private async Task InsertWerkingsgebieden()
    {
        var documentStore = AdminApiHost.DocumentStore();

        await using var session = documentStore.LightweightSession();

        var werkingsgebieden = WerkingsgebiedenServiceMock.All
                                                          .Where(w => w.Code.Length > 8) // only detailed werkingsgebieden
                                                          .Select((w, index) =>
                                                           {
                                                               var nuts = w.Code.Substring(0, 5);
                                                               var lau = w.Code.Substring(5);
                                                               var postcode = (1000 + index).ToString();

                                                               return new PostalNutsLauInfo
                                                               {
                                                                   Postcode = postcode,
                                                                   Gemeentenaam = w.Naam,
                                                                   Nuts = nuts,
                                                                   Lau = lau
                                                               };
                                                           });

        session.StoreObjects(werkingsgebieden);
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

    public async Task DisposeAsync()
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

    public async Task ExecuteGiven(IScenario scenario)
    {
        var documentStore = AdminApiHost.DocumentStore();

        await using var session = documentStore.LightweightSession();
        session.SetHeader(MetadataHeaderNames.Initiator, value: "metadata.Initiator");
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(new Instant()));
        session.CorrelationId = Guid.NewGuid().ToString();

        var givenEvents = await scenario.GivenEvents(AdminApiHost.Services.GetRequiredService<IVCodeService>());

        foreach (var eventsPerStream in givenEvents)
        {
            session.Events.Append(eventsPerStream.Key, eventsPerStream.Value);
        }

        if (givenEvents.Length == 0)
            return;

        await session.SaveChangesAsync();

        await AdminProjectionHost.DocumentStore().WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(30));
        await PublicProjectionHost.DocumentStore().WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(30));
        await AcmApiHost.DocumentStore().WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(30));
    }

    private readonly Dictionary<string, object> _ranContexts = new();

    public void RegisterContext<T>(ITestContext<T> context)
    {

        if (!_ranContexts.TryGetValue(context.GetType().Name, out var ranContext))
        {
            context.Init().GetAwaiter().GetResult();
            _ranContexts.Add(context.GetType().Name, context.RequestResult);
        }
        else
        {
            context.RequestResult = (RequestResult<T>)ranContext;
        }
    }
}
