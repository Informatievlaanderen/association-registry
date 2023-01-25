namespace AssociationRegistry.Test.Public.Api.When_handling_an_event;

using System.Text.RegularExpressions;
using AssociationRegistry.Framework;
using Events;
using Fixtures;
using Framework;
using FluentAssertions;
using Xunit;

public class Unhandled_event_and_one_vereniging_werd_geregistreerd_fixture : PublicApiFixture
{
    public const string VCode = "V0001001";
    public const string Naam = "Feestcommittee Oudenaarde";
    private const string KorteNaam = "FOud";

    private static readonly VerenigingWerdGeregistreerd.Locatie gemeentehuis = new("Gemeentehuis", "dorpstraat", "1", "1b", "9636", "Oudenaarde", "Belgie", false, "Correspondentie");
    private static readonly VerenigingWerdGeregistreerd.Locatie feestzaal = new("Feestzaal", "kerkstraat", "42", null, "9636", "Oudenaarde", "Belgie", true, "Activiteiten");

    public Unhandled_event_and_one_vereniging_werd_geregistreerd_fixture() : base(nameof(Unhandled_event_and_one_vereniging_werd_geregistreerd_fixture))
    {
    }

    public override async Task InitializeAsync()
    {
        await AddEvent(VCode, new EenEvent());
        await AddEvent(
            VCode,
            VerenigingWerdGeregistreerd(
                VCode,
                Naam,
                KorteNaam));
    }

    private static VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd(string vCode, string naam, string? korteNaam)
        => new(vCode,
            naam,
            korteNaam,
            null,
            null,
            null,
            Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
            new[] { gemeentehuis, feestzaal });
}

public record EenEvent : IEvent;

public class Given_an_unhandled_event_and_one_vereniging_werd_geregistreerd : IClassFixture<Unhandled_event_and_one_vereniging_werd_geregistreerd_fixture>
{
    private readonly string _goldenMasterWithOneVereniging;
    private readonly PublicApiClient _publicApiClient;

    private const string EmptyVerenigingenResponse = "{\"verenigingen\": [], \"facets\": {\"hoofdactiviteiten\":[]}, \"metadata\": {\"pagination\": {\"totalCount\": 0,\"offset\": 0,\"limit\": 50}}}";

    public Given_an_unhandled_event_and_one_vereniging_werd_geregistreerd(Unhandled_event_and_one_vereniging_werd_geregistreerd_fixture classFixture)
    {
        _publicApiClient = classFixture.PublicApiClient;
        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_an_unhandled_event_and_one_vereniging_werd_geregistreerd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(Unhandled_event_and_one_vereniging_werd_geregistreerd_fixture.Naam)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var response = await _publicApiClient.Search(Unhandled_event_and_one_vereniging_werd_geregistreerd_fixture.Naam);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", Unhandled_event_and_one_vereniging_werd_geregistreerd_fixture.Naam);
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_name()
    {
        var response = await _publicApiClient.Search("dena");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(EmptyVerenigingenResponse);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_part_of_its_name_when_using_wildcards()
    {
        var response = await _publicApiClient.Search("*dena*");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", "*dena*");
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_full_term_within_its_name()
    {
        var response = await _publicApiClient.Search("oudenaarde");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", "oudenaarde");
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var response = await _publicApiClient.Search(Unhandled_event_and_one_vereniging_werd_geregistreerd_fixture.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", Unhandled_event_and_one_vereniging_werd_geregistreerd_fixture.VCode);
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_vCode()
    {
        var response = await _publicApiClient.Search("001");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(EmptyVerenigingenResponse);
    }

    [Fact]
    public async Task? When_Navigating_To_A_Hoofdactiviteit_Facet_Then_it_is_retrieved()
    {
        var response = await _publicApiClient.Search("*dena*");
        var content = await response.Content.ReadAsStringAsync();

        var regex = new Regex(@"""facets"":\s*{\s*""hoofdactiviteiten"":(.|\s)*?""query"":"".*?(\/v1\/.+?)""");
        var regexResult = regex.Match(content);
        var urlFromFacets = regexResult.Groups[2].Value;

        var responseFromFacetsUrl = await _publicApiClient.HttpClient.GetAsync(urlFromFacets);
        var contentFromFacetsUrl = await responseFromFacetsUrl.Content.ReadAsStringAsync();

        const string expectedUrl = "/v1/verenigingen/zoeken?q=*dena*&facets.hoofdactiviteiten=BWWC";
        contentFromFacetsUrl.Should().Contain(expectedUrl);
    }
}
