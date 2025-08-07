namespace AssociationRegistry.Test.Admin.Api.Framework.Fixtures;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.ElasticSearch;
using AssociationRegistry.Admin.ProjectionHost.Projections.Search;
using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.Test.Admin.Api.Framework.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;
using Xunit;

public abstract class ElasticRepositoryFixture : IDisposable, IAsyncLifetime
{
    private readonly string _identifier;
    private readonly IConfigurationRoot _configurationRoot;
    public ElasticsearchClient? ElasticClient;
    public TypeMapping? TypeMapping;
    public ElasticSearchOptionsSection? ElasticSearchOptions;

    public string VerenigingenIndexName
        => _configurationRoot["ElasticClientOptions:Indices:Verenigingen"];

    public string DuplicateDetectionIndexName
        => _configurationRoot["ElasticClientOptions:Indices:DuplicateDetection"];

    protected ElasticRepositoryFixture(string identifier)
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

        await ConfigureElasticClient(ElasticClient, VerenigingenIndexName, DuplicateDetectionIndexName);

        await InsertDocuments();

        await ElasticClient.Indices.RefreshAsync(Indices.All);
    }

    protected virtual async Task InsertDocuments()
    {
    }

    public async Task IndexDocument(VerenigingZoekDocument document)
    {
        var indexResponse = await ElasticClient!.IndexAsync(document);

        if (!indexResponse.IsValidResponse)
            throw new Exception($"Indexing failed: {indexResponse.DebugInformation}");
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
        //               .DefaultMappingFor(
        //                    typeof(DuplicateDetectionDocument),
        //                    selector: descriptor => descriptor.IndexName(DuplicateDetectionIndexName))
        //               .EnableDebugMode();
        //
        // return new ElasticClient(settings);
    }

    private async Task ConfigureElasticClient(ElasticsearchClient client, string verenigingenIndexName, string duplicateDetectionIndexName)
    {
        if ((await client.Indices.ExistsAsync(verenigingenIndexName)).Exists)
            await client.Indices.DeleteAsync(verenigingenIndexName);

        if ((await client.Indices.ExistsAsync(duplicateDetectionIndexName)).Exists)
            await client.Indices.DeleteAsync(duplicateDetectionIndexName);

        var indexCreation = await client.CreateVerenigingIndexAsync(verenigingenIndexName);
        if (!indexCreation.IsValidResponse)
            throw new InvalidOperationException($"Index creation failed with error {indexCreation.DebugInformation}");

        var duplicateDetectionIndexCreation  = await client.CreateDuplicateDetectionIndexAsync(duplicateDetectionIndexName);
        if (!duplicateDetectionIndexCreation.IsValidResponse)
            throw new InvalidOperationException($"Index creation failed with error {duplicateDetectionIndexCreation.DebugInformation}");


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
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] += _identifier;
        tempConfiguration["ElasticClientOptions:Indices:DuplicateDetection"] += _identifier;

        return tempConfiguration;
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        ElasticClient?.Indices.Delete(VerenigingenIndexName);
        ElasticClient?.Indices.Delete(DuplicateDetectionIndexName);
        ElasticClient?.Indices.Refresh(Indices.All);
    }
}
