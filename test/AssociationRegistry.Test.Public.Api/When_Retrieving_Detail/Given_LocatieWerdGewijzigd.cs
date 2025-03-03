namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using AssociationRegistry.Public.Api.Constants;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using templates;
using Vereniging;
using Vereniging.Verenigingstype;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_LocatieWerdGewijzigd
{
    private readonly PublicApiClient _publicApiClient;
    private readonly V013_LocatieWerdGewijzigdScenario _scenario;

    public Given_LocatieWerdGewijzigd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V013LocatieWerdGewijzigdScenario;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.GetDetail(_scenario.VCode))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_json_ld_as_content_type()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);

        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .WithVCode(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode)
                          .WithType(Verenigingstype.FeitelijkeVereniging)
                          .WithNaam(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam)
                          .WithDoelgroep(_scenario.VCode)
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        goldenMaster.WithLocatie(
            _scenario.VCode,
            _scenario.LocatieWerdGewijzigd.Locatie.LocatieId.ToString(),
            _scenario.LocatieWerdGewijzigd.Locatie.Locatietype,
            _scenario.LocatieWerdGewijzigd.Locatie.Naam,
            _scenario.LocatieWerdGewijzigd.Locatie.AdresId.Broncode,
            _scenario.LocatieWerdGewijzigd.Locatie.AdresId.Bronwaarde,
            _scenario.LocatieWerdGewijzigd.Locatie.IsPrimair);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
