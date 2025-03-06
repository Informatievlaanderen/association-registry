namespace AssociationRegistry.Test.Public.Api.When_Saving_A_Document_To_Elastic;

using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using AssociationRegistry.Public.Schema.Search;
using Framework.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using System.Reflection;
using Xunit;

public abstract class ElasticRepositoryFixture : IDisposable, IAsyncLifetime
{
    private readonly string _identifier;
    private readonly IConfigurationRoot _configurationRoot;
    public IElasticClient? ElasticClient;
    public ElasticRepository? ElasticRepository { get; private set; }

    public string VerenigingenIndexName
        => _configurationRoot["ElasticClientOptions:Indices:Verenigingen"];

    protected ElasticRepositoryFixture(string identifier)
    {
        _identifier += "_" + identifier.ToLowerInvariant();
        _configurationRoot = GetConfiguration();
    }

    public async ValueTask InitializeAsync()
    {
        ElasticClient = CreateElasticClient(_configurationRoot);

        await WaitFor.ElasticSearchToBecomeAvailable(ElasticClient,
                                                     LoggerFactory.Create(opt => opt.AddConsole())
                                                                  .CreateLogger("waitForElasticSearchTestLogger"));

        ConfigureElasticClient(ElasticClient, VerenigingenIndexName);

        ElasticRepository = new ElasticRepository(ElasticClient);
    }

    private IElasticClient CreateElasticClient(IConfiguration configurationRoot)
    {
        var settings = new ConnectionSettings(new Uri(configurationRoot["ElasticClientOptions:Uri"]))
                      .BasicAuthentication(
                           configurationRoot["ElasticClientOptions:Username"],
                           configurationRoot["ElasticClientOptions:Password"])
                      .DefaultMappingFor(
                           typeof(VerenigingZoekDocument),
                           selector: descriptor => descriptor.IndexName(VerenigingenIndexName))
                      .EnableDebugMode();

        return new ElasticClient(settings);
    }

    private void ConfigureElasticClient(IElasticClient client, string verenigingenIndexName)
    {
        if (client.Indices.Exists(verenigingenIndexName).Exists)
            client.Indices.Delete(verenigingenIndexName);

        client.Indices.Create(
            VerenigingenIndexName,
            selector: c => c
               .Map<VerenigingZoekDocument>(
                    m => m
                       .AutoMap<VerenigingZoekDocument>()));

        client.Indices.Refresh(Indices.All);
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
