namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

using System.Reflection;
using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.SearchVerenigingen;
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

        var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
            .BasicAuthentication("elastic", "local_development")
            .DefaultIndex(VerenigingenIndexName);

        ElasticClient = new ElasticClient(settings);
        ElasticClient.Indices.Create(
            VerenigingenIndexName,
            c => c
                .Map<VerenigingDocument>(
                    m => m
                        .AutoMap<VerenigingDocument>()));


        // GIVEN
        var esEventHandler = new ElasticEventHandler(ElasticClient);
        esEventHandler.HandleEvent(new VerenigingWerdGeregistreerd(VCode, Naam)); // TODO cleanup db

        // Make sure all documents are properly indexed
        ElasticClient.Indices.Refresh();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        HttpClient.Dispose();
        _testServer.Dispose();

        ElasticClient.Indices.Delete(VerenigingenIndexName);
    }
}
