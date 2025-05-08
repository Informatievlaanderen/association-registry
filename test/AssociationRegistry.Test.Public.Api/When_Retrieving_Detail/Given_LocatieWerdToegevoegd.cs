namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using AssociationRegistry.Public.Api.Constants;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using templates;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class Given_LocatieWerdToegevoegd
{
    private readonly PublicApiClient _publicApiClient;
    private readonly HttpResponseMessage _response;
    private readonly V011_LocatieWerdToegevoegdScenario _scenario;

    public Given_LocatieWerdToegevoegd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V011LocatieWerdToegevoegdScenario;
    }

    [Fact]
    public async ValueTask Then_we_get_a_successful_response()
        => (await _publicApiClient.GetDetail(_scenario.VCode))
          .Should().BeSuccessful();

    [Fact]
    public async ValueTask Then_we_get_json_ld_as_content_type()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async ValueTask Then_we_get_a_detail_vereniging_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);

        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                          .WithLocatie(
                               _scenario.VCode,
                               _scenario.LocatieWerdToegevoegd.Locatie.LocatieId.ToString(),
                               _scenario.LocatieWerdToegevoegd.Locatie.Locatietype,
                               _scenario.LocatieWerdToegevoegd.Locatie.Naam,
                               _scenario.LocatieWerdToegevoegd.Locatie.AdresId.Broncode,
                               _scenario.LocatieWerdToegevoegd.Locatie.AdresId.Bronwaarde,
                               _scenario.LocatieWerdToegevoegd.Locatie.IsPrimair)
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
