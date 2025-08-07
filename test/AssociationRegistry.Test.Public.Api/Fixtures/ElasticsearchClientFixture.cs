namespace AssociationRegistry.Test.Public.Api.Fixtures;

using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.Schema.Search;
using Framework.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;
using Xunit;

public abstract class ElasticsearchClientFixture : IAsyncLifetime, IDisposable
{
    private readonly string _identifier;
    private readonly IConfigurationRoot _configurationRoot;
    public ElasticsearchClient? ElasticClient;
    public TypeMapping? TypeMapping;
    public ElasticSearchOptionsSection? ElasticSearchOptions;

    public string VerenigingenIndexName
        => _configurationRoot["ElasticClientOptions:Indices:Verenigingen"];

    protected ElasticsearchClientFixture(string identifier)
    {
        _identifier += "_" + identifier.ToLowerInvariant();
        _configurationRoot = GetConfiguration();
    }

    public async ValueTask InitializeAsync()
    {
        ElasticSearchOptions = _configurationRoot.GetSection(ElasticSearchOptionsSection.SectionName).Get<ElasticSearchOptionsSection>();
        ElasticClient = CreateElasticClient(ElasticSearchOptions);

        await WaitFor.ElasticSearchToBecomeAvailable(ElasticClient,
                                                     LoggerFactory.Create(opt => opt.AddConsole())
                                                                  .CreateLogger("waitForElasticSearchTestLogger"));

        await ConfigureElasticClient(ElasticClient, VerenigingenIndexName);

        await InsertDocuments();

        await ElasticClient.Indices.RefreshAsync(Indices.All);
    }

    protected virtual async Task InsertDocuments()
    {
    }

    public async Task IndexDocument(VerenigingZoekDocument document)
    {
        var resp = await ElasticClient!.IndexAsync(document, d => d
                                                                 .Index(VerenigingenIndexName)         // be explicit about the index/alias
                                                                 .Id(document.VCode)                   // <- deterministic id
                                                                 .Refresh(Refresh.WaitFor));           // helpful in tests
        if (!resp.IsValidResponse)
            throw new Exception($"Indexing failed: {resp.DebugInformation}");
    }

    private ElasticsearchClient CreateElasticClient(ElasticSearchOptionsSection options)
    {
        return ElasticSearchExtensions.CreateElasticClient(options, NullLogger.Instance);
        // var settings = new ConnectionSettings(new Uri(configurationRoot["ElasticClientOptions:Uri"]))
        //               .BasicAuthentication(
        //                    configurationRoot["ElasticClientOptions:Username"],
        //                    configurationRoot["ElasticClientOptions:Password"])
        //               .DefaultMappingFor(
        //                    typeof(VerenigingZoekDocument),
        //                    selector: descriptor => descriptor.IndexName(VerenigingenIndexName))
        //               .EnableDebugMode();
        //
        // return new ElasticClient(settings);
    }

    private async Task ConfigureElasticClient(ElasticsearchClient client, string verenigingenIndexName)
    {
        if ((await client.Indices.ExistsAsync(verenigingenIndexName)).Exists)
            await client.Indices.DeleteAsync(verenigingenIndexName);

        await client.CreateVerenigingIndexAsync(verenigingenIndexName);
        var index = (await ElasticClient.Indices.GetAsync(Indices.Index<VerenigingZoekDocument>())).Indices.First();

        TypeMapping = index.Value.Mappings;

        await client.Indices.RefreshAsync(Indices.All);
    }

    private IConfigurationRoot GetConfiguration()
    {
        var maybeRootDirectory = Directory
                                .GetParent(typeof(Program).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;

        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        var builder = new ConfigurationBuilder()
                     .SetBasePath(rootDirectory)
                     .AddJsonFile(path: "appsettings.json", optional: true)
                     .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        var tempConfiguration = builder.Build();
        tempConfiguration["PostgreSQLOptions:database"] += _identifier;
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] += _identifier;

        return tempConfiguration;
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        ElasticClient?.Indices.Delete(VerenigingenIndexName);
        ElasticClient?.Indices.Refresh(Indices.All);
    }
}
