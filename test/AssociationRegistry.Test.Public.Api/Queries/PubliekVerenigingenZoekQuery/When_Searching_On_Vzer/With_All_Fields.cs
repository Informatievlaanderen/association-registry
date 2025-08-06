namespace AssociationRegistry.Test.Public.Api.Queries.PubliekVerenigingenZoekQuery.When_Searching_On_Vzer;

using AssociationRegistry.Public.Api.Constants;
using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.WebApi.Verenigingen.Search.RequestModels;
using AssociationRegistry.Public.Schema.Constants;
using AssociationRegistry.Public.Schema.Search;
using AutoFixture;
using Common.AutoFixture;
using Fixtures;
using FluentAssertions;
using Nest;
using Xunit;

public class SearchingOnDocumentTestsFixture : ElasticsearchClientFixture, IAsyncLifetime
{
    public VerenigingZoekDocument Document { get; private set; }

    public Fixture Fixture { get; private set; }

    public SearchingOnDocumentTestsFixture() : base(nameof(SearchingOnDocumentTestsFixture))
    {
        Fixture = new Fixture().CustomizeAdminApi();
    }

    protected override async Task InsertDocuments()
    {
        Document = CreateFeitelijkeVerenigingDocument(Fixture);

        await IndexDocument(Document);
    }

    private VerenigingZoekDocument CreateFeitelijkeVerenigingDocument(Fixture fixture)
    {
        var doc = fixture.Create<VerenigingZoekDocument>();
        doc.VCode = "V0001001"; // can stay fixed if inserted once
        doc.Naam = "Feestcommittee Oudenaarde";
        doc.HoofdactiviteitenVerenigingsloket = [new() { Code = "BLA", Naam = "Buitengewoon Leuke Afkortingen" }];
        doc.Werkingsgebieden = [
            new() { Code = "BE25", Naam = "Provincie West-Vlaanderen" },
            new() { Code = "BE25535003", Naam = "Bredene" }
        ];
        doc.IsDubbel = false;
        doc.IsVerwijderd = false;
        doc.IsUitgeschrevenUitPubliekeDatastroom = false;
        doc.Status = VerenigingStatus.Actief;
        doc.Locaties =
        [
            new VerenigingZoekDocument.Types.Locatie
            {
                Locatietype = "Activiteiten",
                Adresvoorstelling =
                    "Sorteerstraat 1, 1079 SorteerGemeente Laboriosam quam quia ipsam recusandae eveniet architecto tempora nihil.2025-06-11T06:31:11.595Z, Sorterië",
                Gemeente =
                    "SorteerGemeente Laboriosam quam quia ipsam recusandae eveniet architecto tempora nihil.2025-06-11T06:31:11.595Z",
                Postcode = "1079",
            },
        ];
        return doc;
    }
}

public class SearchingOnDocumentTests : IClassFixture<SearchingOnDocumentTestsFixture>
{
    private readonly IElasticClient _elasticClient;
    private readonly ITypeMapping _typeMapping;
    private readonly VerenigingZoekDocument _document;

    public SearchingOnDocumentTests(SearchingOnDocumentTestsFixture fixture)
    {
        _elasticClient = fixture.ElasticClient;
        _typeMapping = fixture.TypeMapping;
        _document = fixture.Document;
    }

    [Fact]
    public async Task Then_vereniging_can_be_retrieved_by_exact_name()
    {
        var response = await ExecuteSearch("Feestcommittee Oudenaarde");
        response.Documents.Should().ContainSingle(d => d.VCode == _document.VCode);
    }

    [Fact]
    public async Task Then_vereniging_is_not_found_by_partial_name()
    {
        var response = await ExecuteSearch("dena");
        response.Documents.Should().NotContain(d => d.VCode == _document.VCode);
    }

    [Fact]
    public async Task Then_vereniging_is_found_by_partial_name_with_wildcards()
    {
        var response = await ExecuteSearch("*dena*");
        response.Documents.Should().ContainSingle(d => d.VCode == _document.VCode);
    }

    [Fact]
    public async Task Then_vereniging_is_found_by_full_term_within_name()
    {
        var response = await ExecuteSearch("oudenaarde");
        response.Documents.Should().ContainSingle(d => d.VCode == _document.VCode);
    }

    [Fact]
    public async Task Then_vereniging_is_found_by_vCode()
    {
        var response = await ExecuteSearch("V0001001");
        response.Documents.Should().ContainSingle(d => d.VCode == _document.VCode);
    }

    [Fact]
    public async Task Then_vereniging_is_not_found_by_partial_vCode()
    {
        var response = await ExecuteSearch("00100");
        response.Documents.Should().NotContain(d => d.VCode == _document.VCode);
    }

    [Fact]
    public async Task Then_vereniging_is_found_by_single_werkingsgebied()
    {
        var response = await ExecuteSearch("werkingsgebieden.code:BE25");
        response.Documents.Should().ContainSingle(d => d.VCode == _document.VCode);
    }

    [Theory]
    [InlineData("NVT")]
    [InlineData("BE2")]
    [InlineData("BE255")]
    public async Task Then_vereniging_is_not_found_by_other_werkingsgebied(string code)
    {
        var response = await ExecuteSearch($"werkingsgebieden.code:{code}");
        response.Documents.Should().NotContain(d => d.VCode == _document.VCode);
    }

    [Fact]
    public async Task Then_vereniging_is_found_by_multiple_werkingsgebieden_or()
    {
        var response = await ExecuteSearch("werkingsgebieden.code:(BE OR BE25 OR BE21 OR BE212)");
        response.Documents.Should().ContainSingle(d => d.VCode == _document.VCode);
    }

    [Fact]
    public async Task Then_vereniging_is_not_found_if_none_of_werkingsgebieden_match()
    {
        var response = await ExecuteSearch("werkingsgebieden.code:(BE OR BE2 OR BE21 OR BE212)");
        response.Documents.Should().NotContain(d => d.VCode == _document.VCode);
    }

    [Fact]
    public async Task Then_vereniging_is_found_if_all_werkingsgebieden_match()
    {
        var response = await ExecuteSearch("werkingsgebieden.code:(BE25 AND BE25535003)");
        response.Documents.Should().ContainSingle(d => d.VCode == _document.VCode);
    }

    [Fact]
    public async Task Then_vereniging_is_not_found_if_not_all_werkingsgebieden_match()
    {
        var response = await ExecuteSearch("werkingsgebieden.code:(BE25 AND BE2)");
        response.Documents.Should().NotContain(d => d.VCode == _document.VCode);
    }

    [Theory]
    [InlineData("locaties.gemeente:\"SorteerGemeente Laboriosam quam quia ipsam recusandae eveniet architecto tempora nihil.2025-06-11T06:31:11.595Z\"")]
    public async Task Then_vereniging_is_found_by_gemeente(string query)
    {
        var response = await ExecuteSearch(query);
        response.Documents.Should().Contain(d => d.VCode == _document.VCode);
    }

    [Fact]
    public async Task When_Navigating_To_A_Hoofdactiviteit_Facet_Then_it_is_retrieved()
    {
        var zoekQuery = new PubliekVerenigingenZoekQuery(_elasticClient, _typeMapping);
        var response = await zoekQuery.ExecuteAsync(
            new ("*", "vCode", [], new PaginationQueryParams()),
            CancellationToken.None);


        response.Documents.Should().ContainSingle(d => d.VCode == _document.VCode);
        response.Aggregations.Should().NotBeNull();

        var globalFacet = response.Aggregations.Filter(WellknownFacets.GlobalAggregateName);
        globalFacet.Should().NotBeNull();

        var globalFilter = globalFacet.Filter(WellknownFacets.FilterAggregateName);
        globalFilter.Should().NotBeNull();

        var hoofdActiviteitenFacet = globalFilter.Terms(WellknownFacets.HoofdactiviteitenCountAggregateName);
        hoofdActiviteitenFacet.Should().NotBeNull();
        hoofdActiviteitenFacet.Buckets.Should().HaveCount(1);

        var blaBucket = hoofdActiviteitenFacet.Buckets.SingleOrDefault(x => x.Key == "BLA");
        blaBucket.Should().NotBeNull();
        blaBucket.DocCount.Should().Be(1);
    }

    private async Task<ISearchResponse<VerenigingZoekDocument>> ExecuteSearch(string query)
    {
        var zoekQuery = new PubliekVerenigingenZoekQuery(_elasticClient, _typeMapping);
        return await zoekQuery.ExecuteAsync(
            new (query, "vCode", [], new PaginationQueryParams()),
            CancellationToken.None);
    }
}
