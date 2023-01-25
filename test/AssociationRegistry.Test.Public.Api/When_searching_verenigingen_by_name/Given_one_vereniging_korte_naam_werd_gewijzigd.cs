namespace AssociationRegistry.Test.Public.Api.When_searching_verenigingen_by_name;

using AutoFixture;
using Events;
using Framework;
using Fixtures;
using FluentAssertions;
using Xunit;

public class Given_one_vereniging_korte_naam_werd_gewijzigd_fixture : PublicApiFixture
{
    private const string VCode = "V0001001";
    public static string Naam = "Feestcommittee Oudenaarde";
    public const string NieuweKorteNaam = "Een nieuwe korte naam";
    private const string KorteNaam = "FOud";

    private static readonly VerenigingWerdGeregistreerd.Locatie Gemeentehuis = new("Gemeentehuis", "dorpstraat", "1", "1b", "9636", "Oudenaarde", "Belgie", false, "Correspondentie");
    private static readonly VerenigingWerdGeregistreerd.Locatie Feestzaal = new("Feestzaal", "kerkstraat", "42", null, "9636", "Oudenaarde", "Belgie", true, "Activiteiten");

    public Given_one_vereniging_korte_naam_werd_gewijzigd_fixture() : base(nameof(Given_one_vereniging_korte_naam_werd_gewijzigd_fixture))
    {
    }

    public override async Task InitializeAsync()
    {
        await AddEvent(
            VCode,
            VerenigingWerdGeregistreerd(
                VCode,
                Naam,
                KorteNaam));
        await AddEvent(
            VCode,
            new KorteNaamWerdGewijzigd(VCode, NieuweKorteNaam));
    }

    private static VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd(string vCode, string naam, string? korteNaam)
        => new(vCode,
            naam,
            korteNaam,
            null,
            null,
            null,
            Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
            new[] { Gemeentehuis, Feestzaal });
}

public class Given_one_vereniging_korte_naam_werd_gewijzigd : IClassFixture<Given_one_vereniging_korte_naam_werd_gewijzigd_fixture>
{
    private readonly string _goldenMasterWithOneVereniging;
    private readonly PublicApiClient _publicApiClient;

    public Given_one_vereniging_korte_naam_werd_gewijzigd(Given_one_vereniging_korte_naam_werd_gewijzigd_fixture classFixture)
    {
        _publicApiClient = classFixture.PublicApiClient;
        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_one_vereniging_korte_naam_werd_gewijzigd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(Given_one_vereniging_korte_naam_werd_gewijzigd_fixture.Naam)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var response = await _publicApiClient.Search(Given_one_vereniging_korte_naam_werd_gewijzigd_fixture.Naam);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", Given_one_vereniging_korte_naam_werd_gewijzigd_fixture.Naam);
        content.Should().BeEquivalentJson(goldenMaster);
    }
}
