namespace AssociationRegistry.Test.Admin.Api.WhenDetectingDuplicates.Playground;

using AssociationRegistry.Admin.Api.DuplicateDetection;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Search;
using AssociationRegistry.Admin.Schema.Search;
using FluentAssertions;
using Framework.Fixtures;
using Hosts;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;
using System.Reflection;
using Vereniging;
using Xunit;

public class ForDuplicateDetection : IClassFixture<DuplicateDetectionSetup>
{
    private readonly ElasticClient _elasticClient;
    private readonly SearchDuplicateVerenigingDetectionService _duplicateDetectionService;

    public ForDuplicateDetection(DuplicateDetectionSetup setup)
    {
        _elasticClient = setup.Client;
        _duplicateDetectionService = setup.DuplicateDetectionService;
    }

    [Fact]
    public async Task It_asciis_the_tokens()
    {
        var analyzeResponse =
            await _elasticClient.Indices
                                .AnalyzeAsync(a => a
                                                  .Index<DuplicateDetectionDocument>()
                                                  .Analyzer(DuplicateDetectionDocumentMapping.DuplicateAnalyzer)
                                                  .Text(
                                                       "Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling")
                                 );

        analyzeResponse
           .Tokens
           .Select(x => x.Token)
           .Should()
           .BeEquivalentTo(
                "vereniging",
                "technologieenthusiasten",
                "inovacie",
                "entwikkeling");
    }

    [Fact]
    public async Task Stopwords_Everywhere()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("De Vereniging van de Technologïeënthusiasten en ook de Inováçie en van de Ëntwikkeling"),
            Met1MatchendeGemeente());

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task Fuzzy()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("De Verengiging vn Technologïeënthusiasten: Inováçie & Ëntwikkeling"),
            Met1MatchendeGemeente());

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    private static Locatie[] Met1MatchendeGemeente()
    {
        return new[]
        {
            Locatie.Create(naam: Locatienaam.Create("xx"), isPrimair: false, Locatietype.Correspondentie, adresId: null,
                           Adres.Create(straatnaam: "xx", huisnummer: "xx", busnummer: "xx", postcode: "xx", gemeente: "Hulste",
                                        land: "xx")),
            Locatie.Create(naam: Locatienaam.Create("xx"), isPrimair: false, Locatietype.Correspondentie, adresId: null,
                                                    Adres.Create(straatnaam: "xx", huisnummer: "xx", busnummer: "xx", postcode: "xx", gemeente: "Kortrijk",
                                                                 land: "xx")),
        };
    }
}

public class DuplicateDetectionSetup
{
    public ElasticClient Client { get; }
    public ElasticRepository Repository { get; }
    public SearchDuplicateVerenigingDetectionService DuplicateDetectionService { get; }

    public DuplicateDetectionSetup()
    {
        var maybeRootDirectory = Directory
                                .GetParent(typeof(AdminApiFixture).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;

        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        var configuration = new ConfigurationBuilder()
                           .SetBasePath(rootDirectory)
                           .AddJsonFile("appsettings.json")
                           .AddJsonFile($"appsettings.{Environment.MachineName.ToLower()}.json", optional: true)
                           .Build();

        var elasticSearchOptions = configuration.GetSection("ElasticClientOptions")
                                                .Get<ElasticSearchOptionsSection>();

        var duplicateDetection = "analyzethis";

        var settings = new ConnectionSettings(new Uri(elasticSearchOptions.Uri!))
                      .BasicAuthentication(
                           elasticSearchOptions.Username,
                           elasticSearchOptions.Password)
                      .MapVerenigingDocument(elasticSearchOptions.Indices!.Verenigingen!)
                      .MapDuplicateDetectionDocument(duplicateDetection);

        Client = new ElasticClient(settings);

        WaitFor.ElasticSearchToBecomeAvailable(Client, NullLogger.Instance, 10, CancellationToken.None)
               .GetAwaiter().GetResult();

        Client.Indices.Delete(duplicateDetection);
        ElasticSearchExtensions.EnsureIndexExists(Client, elasticSearchOptions.Indices.Verenigingen!, duplicateDetection);

        Repository = new ElasticRepository(Client);

        DuplicateDetectionService = new SearchDuplicateVerenigingDetectionService(Client);

        Repository.Index(new DuplicateDetectionDocument
        {
            VCode = "V0001001",
            Naam = "Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling",
            VerenigingsTypeCode = Verenigingstype.FeitelijkeVereniging.Code,
            Locaties = new[]
            {
                new DuplicateDetectionDocument.Locatie
                {
                    Gemeente = "Hulste",
                    Postcode = "8531",
                },
            },
        });
    }
}
