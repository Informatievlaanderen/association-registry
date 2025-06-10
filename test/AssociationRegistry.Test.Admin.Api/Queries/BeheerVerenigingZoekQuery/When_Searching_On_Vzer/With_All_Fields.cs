namespace AssociationRegistry.Test.Admin.Api.Queries.BeheerVerenigingZoekQuery.When_Searching_On_Vzer;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Admin.Schema.Search;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Framework.Fixtures;
using Nest;
using Public.Schema.Constants;
using Xunit;

public class SearchingOnDocumentTestsFixture : ElasticRepositoryFixture
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
        doc.Sequence = 999;
        return doc;
    }
}

public class With_All_Fields : IClassFixture<SearchingOnDocumentTestsFixture>
{
    private readonly IElasticClient _elasticClient;
    private readonly ITypeMapping _typeMapping;
    private readonly VerenigingZoekDocument _document;

    public With_All_Fields(SearchingOnDocumentTestsFixture fixture, ITestOutputHelper helper)
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
    public async Task Then_vereniging_is_not_found_by_sequence()
    {
        var response = await ExecuteSearch($"sequence:{_document.Sequence.ToString()}");
        response.Documents.Should().NotContain(d => d.VCode == _document.VCode);
    }

    [Fact]
    public async Task Then_vereniging_is_not_found_by_sequence_field()
    {
        var response = await ExecuteSearch($"sequence:{_document.Sequence.ToString()}");
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

    private async Task<ISearchResponse<VerenigingZoekDocument>> ExecuteSearch(string query)
    {
        var zoekQuery = new BeheerVerenigingenZoekQuery(_elasticClient, _typeMapping);
        return await zoekQuery.ExecuteAsync(
            new (query, "vCode", new PaginationQueryParams()),
            CancellationToken.None);
    }
}

