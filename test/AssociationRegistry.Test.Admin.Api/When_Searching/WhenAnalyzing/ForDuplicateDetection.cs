namespace AssociationRegistry.Test.Admin.Api.When_Searching.WhenAnalyzing;

using AssociationRegistry.Admin.Api.DuplicateDetection;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Search;
using AssociationRegistry.Admin.Schema.Search;
using Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Nest;
using System.Reflection;
using Vereniging;
using Xunit;
using ElasticSearchExtensions = AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions.ElasticSearchExtensions;

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
        var analyzeResponse = await _elasticClient.Indices.AnalyzeAsync(a => a
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
    public async Task By_Exact_Name()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling"),
            Met1MatchendeGemeente());

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task By_Broad_Term_Vereniging()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Mijn Vereniging"),
            Array.Empty<Locatie>());

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task By_Broad_Term_Vereniging_X()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vereniging X"),
            Array.Empty<Locatie>());

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task By_Broad_Term_Vereniging_X_ButPostcodeOfOther()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vereniging X"),
            new Locatie[]
            {
                Locatie.Create("", false, Locatietype.Correspondentie, null, Adres.Create("xx", "xx", "xx", "8531", "xx", "BE"))
            });

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task By_Unrelated_Term_But_PostcodeOfOther()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Mijn Huisje"),
            new Locatie[]
            {
                Locatie.Create("", false, Locatietype.Correspondentie, null, Adres.Create("xx", "xx", "xx", "8531", "xx", "BE"))
            });

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }


    [Fact]
    public async Task By_Broad_Term_Vereniging_X_ButGemeenteOfOther()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vereniging X"),
            new Locatie[]
            {
                Locatie.Create("", false, Locatietype.Correspondentie, null, Adres.Create("xx", "xx", "xx", "1000", "Hulste", "BE"))
            });

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task By_Partial_Name()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vereniging van Technologïeënthusiasten: Inováçie"),
            Met1MatchendeGemeente());

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task By_Partial_Name_Ending()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Technologïeënthusiasten: Inováçie & Ëntwikkeling"),
            Met1MatchendeGemeente());

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task Spaces_Everywhere()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create(" Technologïeënthusiasten  :    Inováçie    &   Ëntwikkeling  "),
            Met1MatchendeGemeente());

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
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

    [Fact]
    public async Task By_Name_Wrong_Punctuation()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Technologïeënthusiasten. Inováçie & Ëntwikkeling?"),
            Met1MatchendeGemeente());

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task By_Name_No_Accents()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vereniging van Technologieenthusiasten: Inovacie & Entwikkeling"),
            Met1MatchendeGemeente());

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task By_Name_More_Accents()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vërëniging van Tëchnologieenthusiasten: Inovacie & Entwikkeling"),
            Met1MatchendeGemeente());

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task By_lowercase_Name()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("vereniging van technologïeënthusiasten: Inováçie & Ëntwikkeling"),
            Met1MatchendeGemeente());

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling");
    }

    [Fact]
    public async Task By_Exact_Name_En_PostCode_En_Gemeente()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vereniging"),
            new[]
            {
                Locatie.Create(naam: "xx", isPrimair: false, Locatietype.Correspondentie, adresId: null,
                               Adres.Create(straatnaam: "xx", huisnummer: "xx", busnummer: "xx", postcode: "8532", gemeente: "Ooigem",
                                            land: "xx")),
            });

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging X");
    }

    [Fact]
    public async Task By_Exact_Name_En_PostCode_En_Not_Gemeente()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vereniging"),
            new[]
            {
                Locatie.Create(naam: "xx", isPrimair: false, Locatietype.Correspondentie, adresId: null,
                               Adres.Create(straatnaam: "xx", huisnummer: "xx", busnummer: "xx", postcode: "8532", gemeente: "Blaarmeersen",
                                            land: "xx")),
            });

        duplicates.Should().HaveCount(1);
        duplicates.Single().Naam.Should().Be("Vereniging X");
    }

    [Fact]
    public async Task By_Exact_Name_En_Almost_PostCode()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vereniging"),
            new[]
            {
                Locatie.Create(naam: "xx", isPrimair: false, Locatietype.Correspondentie, adresId: null,
                               Adres.Create(straatnaam: "xx", huisnummer: "xx", busnummer: "xx", postcode: "853", gemeente: "xx",
                                            land: "xx")),
            });

        duplicates.Should().HaveCount(0);
    }

    [Fact]
    public async Task By_Exact_Name_En_Fuzzy_PostCode()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vereniging"),
            new[]
            {
                Locatie.Create(naam: "xx", isPrimair: false, Locatietype.Correspondentie, adresId: null,
                               Adres.Create(straatnaam: "xx", huisnummer: "xx", busnummer: "xx", postcode: "8351", gemeente: "xx",
                                            land: "xx")),
            });

        duplicates.Should().HaveCount(0);
    }

    [Fact]
    public async Task By_Exact_Name_En_Almost_Gemeente()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vereniging"),
            new[]
            {
                Locatie.Create(naam: "xx", isPrimair: false, Locatietype.Correspondentie, adresId: null,
                               Adres.Create(straatnaam: "xx", huisnummer: "xx", busnummer: "xx", postcode: "8000", gemeente: "ooige",
                                            land: "xx")),
            });

        duplicates.Should().HaveCount(1);
        duplicates.Single().Locaties.Single().Gemeente.Should().Be("Ooigem");
    }

    [Fact]
    public async Task By_Exact_Name_En_Fuzzy_Gemeente()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        var duplicates = await _duplicateDetectionService.GetDuplicates(
            VerenigingsNaam.Create("Vereniging"),
            new[]
            {
                Locatie.Create(naam: "xx", isPrimair: false, Locatietype.Correspondentie, adresId: null,
                               Adres.Create(straatnaam: "xx", huisnummer: "xx", busnummer: "xx", postcode: "8000", gemeente: "oiiogem",
                                            land: "xx")),
            });

        duplicates.Should().HaveCount(1);
        duplicates.Single().Locaties.Single().Gemeente.Should().Be("Ooigem");
    }

    private static Locatie[] Met1MatchendeGemeente()
    {
        return new[]
        {
            Locatie.Create(naam: "xx", isPrimair: false, Locatietype.Correspondentie, adresId: null,
                           Adres.Create(straatnaam: "xx", huisnummer: "xx", busnummer: "xx", postcode: "xx", gemeente: "Hulste",
                                        land: "xx")),
            Locatie.Create(naam: "xx", isPrimair: false, Locatietype.Correspondentie, adresId: null,
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
                           .AddJsonFile($"appsettings.{Environment.MachineName.ToLower()}.json")
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
        Client.Indices.Delete(duplicateDetection);
        ElasticSearchExtensions.EnsureIndexExists(Client, elasticSearchOptions.Indices.Verenigingen!, duplicateDetection);

        Repository = new ElasticRepository(Client);

        DuplicateDetectionService = new SearchDuplicateVerenigingDetectionService(Client);

        Repository.Index(new DuplicateDetectionDocument
        {
            VCode = "V0001001",
            Naam = "Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling",
            Locaties = new[]
            {
                new VerenigingZoekDocument.Locatie
                {
                    Gemeente = "Hulste",
                    Postcode = "8531",
                },
            },
        });

        Repository.Index(new DuplicateDetectionDocument
        {
            VCode = "V0001002",
            Naam = "Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling",
            Locaties = new[]
            {
                new VerenigingZoekDocument.Locatie
                {
                    Gemeente = "Bavikhove",
                    Postcode = "8531",
                },
            },
        });

        Repository.Index(new DuplicateDetectionDocument
        {
            VCode = "V0001003",
            Naam = "Vereniging X",
            Locaties = new[]
            {
                new VerenigingZoekDocument.Locatie
                {
                    Gemeente = "Ooigem",
                    Postcode = "8532",
                },
            },
        });

        Repository.Index(new DuplicateDetectionDocument
        {
            VCode = "V0001004",
            Naam = "Vereniging Y",
            Locaties = new[]
            {
                new VerenigingZoekDocument.Locatie
                {
                    Gemeente = "Alveringem",
                    Postcode = "8533",
                },
            },
        });

        Repository.Index(new DuplicateDetectionDocument
        {
            VCode = "V0001005",
            Naam = "Vereniging Z",
            Locaties = new[]
            {
                new VerenigingZoekDocument.Locatie
                {
                    Gemeente = "Poperinge",
                    Postcode = "8513",
                },
            },
        });

        Repository.Index(new DuplicateDetectionDocument
        {
            VCode = "V0001006",
            Naam = "Werenigung Z",
            Locaties = new[]
            {
                new VerenigingZoekDocument.Locatie
                {
                    Gemeente = "Poperinge",
                    Postcode = "8513",
                },
            },
        });
    }
}
