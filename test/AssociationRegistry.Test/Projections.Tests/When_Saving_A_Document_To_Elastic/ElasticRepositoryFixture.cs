namespace AssociationRegistry.Test.Projections.Tests.When_Saving_A_Document_To_Elastic;

using System.Reflection;
using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.Projections;
using AssociationRegistry.Public.Api.SearchVerenigingen;
using Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using Xunit;

public abstract class ElasticRepositoryFixture : IDisposable, IAsyncLifetime
{
    private readonly string _identifier;
    private readonly IConfigurationRoot _configurationRoot;
    private readonly IElasticClient _elasticClient;
    public ElasticRepository ElasticRepository { get; }

    private string VerenigingenIndexName
        => _configurationRoot["ElasticClientOptions:Indices:Verenigingen"];

    protected ElasticRepositoryFixture(string identifier)
    {
        _identifier += "_" + identifier.ToLowerInvariant();
        GoToRootDirectory();

        _configurationRoot = SetConfigurationRoot();

        _elasticClient = CreateElasticClient();

        ElasticRepository = new ElasticRepository(_elasticClient);
    }

    private static void GoToRootDirectory()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Startup).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");
        Directory.SetCurrentDirectory(rootDirectory);
    }

    private IConfigurationRoot SetConfigurationRoot()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);
        var tempConfiguration = builder.Build();
        tempConfiguration["PostgreSQLOptions:database"] += _identifier;
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] += _identifier;
        return tempConfiguration;
    }


    private IElasticClient CreateElasticClient()
    {
        var settings = new ConnectionSettings(new Uri(_configurationRoot["ElasticClientOptions:Uri"]))
            .BasicAuthentication(
                _configurationRoot["ElasticClientOptions:Username"],
                _configurationRoot["ElasticClientOptions:Password"])
            .DefaultMappingFor(
                typeof(VerenigingDocument),
                descriptor => descriptor.IndexName(VerenigingenIndexName))
            .EnableDebugMode();

        return new ElasticClient(settings);
    }

    private void ConfigureElasticClient(IElasticClient client, string verenigingenIndexName)
    {
        if (client.Indices.Exists(verenigingenIndexName).Exists)
            client.Indices.Delete(verenigingenIndexName);

        client.Indices.Create(
            VerenigingenIndexName,
            c => c
                .Map<VerenigingDocument>(
                    m => m
                        .AutoMap<VerenigingDocument>()));
        client.Indices.Refresh(Indices.All);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _elasticClient.Indices.Delete(VerenigingenIndexName);
        _elasticClient.Indices.Refresh(Indices.All);
    }

    public async Task InitializeAsync()
    {
        await WaitFor.ElasticSearchToBecomeAvailable(_elasticClient, LoggerFactory.Create(opt => opt.AddConsole()).CreateLogger("waitForElasticSearchTestLogger"));
        ConfigureElasticClient(_elasticClient, VerenigingenIndexName);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
