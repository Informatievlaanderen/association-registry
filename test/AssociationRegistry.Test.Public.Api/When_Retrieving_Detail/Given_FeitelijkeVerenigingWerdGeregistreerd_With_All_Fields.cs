namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using AssociationRegistry.Public.Api.Constants;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields
{
    private readonly PublicApiClient _publicApiClient;
    private readonly V001_FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V001FeitelijkeVerenigingWerdGeregistreerdScenario;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);
        response.Should().BeSuccessful();
    }

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
                          .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        foreach (var werkingsgebied in _scenario.WerkingsgebiedenWerdenBepaald.Werkingsgebieden)
        {
            goldenMaster.WithWerkingsgebied(werkingsgebied.Code, werkingsgebied.Naam);
        }

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
