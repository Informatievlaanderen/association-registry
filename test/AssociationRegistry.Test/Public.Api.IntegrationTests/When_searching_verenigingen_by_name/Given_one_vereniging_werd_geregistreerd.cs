namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_searching_verenigingen_by_name;

using System.Text.RegularExpressions;
using Fixtures;
using FluentAssertions;
using Vereniging;
using Xunit;

public class One_vereniging_werd_geregistreerd_fixture : PublicApiFixture
{
    public const string VCode = "V0001001";
    public const string Naam = "Feestcommittee Oudenaarde";
    private const string KorteNaam = "FOud";

    public One_vereniging_werd_geregistreerd_fixture() : base(nameof(One_vereniging_werd_geregistreerd_fixture))
    {
    }

    public override async Task InitializeAsync()
        => await AddEvent(
            VCode,
            new VerenigingWerdGeregistreerd(
                VCode,
                Naam,
                KorteNaam
            ) { DatumLaatsteAanpassing = DateOnly.MinValue });
}

public class Given_one_vereniging_werd_geregistreerd : IClassFixture<One_vereniging_werd_geregistreerd_fixture>
{
    private readonly One_vereniging_werd_geregistreerd_fixture _classFixture;
    private readonly string _goldenMasterWithOneVereniging;

    private const string VerenigingenZoekenOpNaam = "/v1/verenigingen/zoeken?q=" + One_vereniging_werd_geregistreerd_fixture.Naam;
    private const string VerenigingenZoekenOpDeelVanEenTermVanDeNaam = "/v1/verenigingen/zoeken?q=dena";
    private const string VerenigingenZoekenOpDeelVanNaamMetWildcards = "/v1/verenigingen/zoeken?q=*dena*";
    private const string VerenigingenZoekenOpTermInNaam = "/v1/verenigingen/zoeken?q=oudenaarde";

    private const string VerenigingenZoekenOpVCode = "/v1/verenigingen/zoeken?q=" + One_vereniging_werd_geregistreerd_fixture.VCode;
    private const string VerenigingenZoekenOpDeelVanDeVCode = "/v1/verenigingen/zoeken?q=001";

    private const string EmptyVerenigingenResponse = "{\"verenigingen\": [], \"facets\": {\"hoofdactiviteiten\":[]}, \"metadata\": {\"pagination\": {\"totalCount\": 0,\"offset\": 0,\"limit\": 50}}}";

    public Given_one_vereniging_werd_geregistreerd(One_vereniging_werd_geregistreerd_fixture classFixture)
    {
        _classFixture = classFixture;
        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_one_vereniging_werd_geregistreerd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _classFixture.GetResponseMessage(VerenigingenZoekenOpNaam)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var content = await _classFixture.Search(VerenigingenZoekenOpNaam);

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", One_vereniging_werd_geregistreerd_fixture.Naam);
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_name()
    {
        var content = await _classFixture.Search(VerenigingenZoekenOpDeelVanEenTermVanDeNaam);

        content.Should().BeEquivalentJson(EmptyVerenigingenResponse);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_part_of_its_name_when_using_wildcards()
    {
        var content = await _classFixture.Search(VerenigingenZoekenOpDeelVanNaamMetWildcards);

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", "*dena*");
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_full_term_within_its_name()
    {
        var content = await _classFixture.Search(VerenigingenZoekenOpTermInNaam);

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", "oudenaarde");
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var content = await _classFixture.Search(VerenigingenZoekenOpVCode);

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", One_vereniging_werd_geregistreerd_fixture.VCode);
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_vCode()
    {
        var content = await _classFixture.Search(VerenigingenZoekenOpDeelVanDeVCode);

        content.Should().BeEquivalentJson(EmptyVerenigingenResponse);
    }

    [Fact]
    public async Task? When_Navigating_To_A_Hoofdactiviteit_Facet_Then_it_is_retrieved()
    {
        const string arbitrarySearchThatReturnsAResult = VerenigingenZoekenOpDeelVanNaamMetWildcards;
        var content = await _classFixture.Search(arbitrarySearchThatReturnsAResult);

        var regex = new Regex(@"""facets"":\s*{\s*""hoofdactiviteiten"":(.|\s)*?""query"":"".*?(\/v1\/.+?)""");
        var regexResult = regex.Match(content);
        var urlFromFacets = regexResult.Groups[2].Value;

        var contentFromFacetsUrl = await _classFixture.Search(urlFromFacets);

        const string expectedUrl = "/v1/verenigingen/zoeken?q=(hoofdactiviteiten.code:BWRK OR hoofdactiviteiten.code:BWRK) AND *dena*";
        contentFromFacetsUrl.Should().Contain(expectedUrl);
    }
}
