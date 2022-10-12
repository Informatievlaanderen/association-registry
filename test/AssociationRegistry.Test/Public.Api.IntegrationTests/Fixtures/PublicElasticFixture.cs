namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

using System.Reflection;
using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.SearchVerenigingen;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using When_searching_verenigingen_by_name;

public class PublicElasticFixture : IDisposable
{
    public const string VCode = "v000001";
    public const string Naam = "Feestcommittee Oudenaarde";

    private const string VerenigingenIndexName = "test-verenigingsregister-verenigingen";
    public HttpClient HttpClient { get; private set; }
    public ElasticClient ElasticClient { get; private set; }
    private readonly TestServer _testServer;

    public PublicElasticFixture()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Startup).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        Directory.SetCurrentDirectory(rootDirectory);

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        var configurationRoot = builder.Build();

        IWebHostBuilder hostBuilder = new WebHostBuilder();

        hostBuilder.UseConfiguration(configurationRoot);
        hostBuilder.UseStartup<Startup>();

        hostBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole());

        hostBuilder.UseTestServer();

        _testServer = new TestServer(hostBuilder);

        HttpClient = _testServer.CreateClient();

        var settings = new ConnectionSettings(new Uri(configurationRoot["ElasticClientOptions:Uri"]))
            .BasicAuthentication(
                configurationRoot["ElasticClientOptions:Username"],
                configurationRoot["ElasticClientOptions:Password"])
            .DefaultMappingFor(
                typeof(VerenigingDocument),
                descriptor => descriptor.IndexName(configurationRoot["ElasticClientOptions:Indices:Verenigingen"]));

        ElasticClient = new ElasticClient(settings);
        ElasticClient.Indices.Create(
            VerenigingenIndexName,
            c => c
                .Map<VerenigingDocument>(
                    m => m
                        .AutoMap<VerenigingDocument>()));

        ElasticClient.Indices.Refresh(Indices.All);

        // GIVEN
        var esEventHandler = new ElasticEventHandler(ElasticClient);
        var store = DocumentStore.For(
            opts =>
            {
                opts.Connection("host=localhost;database=verenigingsregister;password=root;username=root");
                opts.Events.StreamIdentity = StreamIdentity.AsString;

                opts.Projections.Add(new MartenSubscription(new MartenEventsConsumer(esEventHandler)), ProjectionLifecycle.Async);
            });

        using var session = store.OpenSession();
        session.Events.Append(VCode, new VerenigingWerdGeregistreerd(VCode, Naam));
        session.SaveChanges();

        var daemon = store.BuildProjectionDaemon();
        daemon.StartAllShards().GetAwaiter().GetResult();
        daemon.WaitForNonStaleData(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();

        // Make sure all documents are properly indexed
        ElasticClient.Indices.Refresh(Indices.All);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        HttpClient.Dispose();
        _testServer.Dispose();

        ElasticClient.Indices.Delete(VerenigingenIndexName);
        ElasticClient.Indices.Refresh(Indices.All);
    }
}

public class MartenSubscription: IProjection
{
    private readonly IMartenEventsConsumer consumer;

    public MartenSubscription(IMartenEventsConsumer consumer)
    {
        this.consumer = consumer;
    }

    public void Apply(
        IDocumentOperations operations,
        IReadOnlyList<StreamAction> streams
    )
    {
        throw new NotSupportedException("Subscription should be only run asynchronously");
    }

    public Task ApplyAsync(
        IDocumentOperations operations,
        IReadOnlyList<StreamAction> streams,
        CancellationToken ct
    )
    {
        return consumer.ConsumeAsync(streams);
    }
}

public interface IMartenEventsConsumer
{
    Task ConsumeAsync(IReadOnlyList<StreamAction> streamActions);
}

public class MartenEventsConsumer: IMartenEventsConsumer
{
    private readonly ElasticEventHandler _eventHandler;

    public MartenEventsConsumer(ElasticEventHandler eventHandler)
    {
        _eventHandler = eventHandler;
    }

    public Task ConsumeAsync(IReadOnlyList<StreamAction> streamActions)
    {
        foreach (var @event in streamActions.SelectMany(streamAction => streamAction.Events))
        {
            if (@event.EventType == typeof(VerenigingWerdGeregistreerd))
            {
                _eventHandler.HandleEvent((VerenigingWerdGeregistreerd)@event.Data);
            }
        }

        return Task.CompletedTask;
    }
}
