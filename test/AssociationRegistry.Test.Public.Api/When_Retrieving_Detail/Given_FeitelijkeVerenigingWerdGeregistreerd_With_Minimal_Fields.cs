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
public class Given_FeitelijkeVerenigingWerdGeregistreerd_With_Minimal_Fields
{
    private readonly PublicApiClient _publicApiClient;
    private readonly V002_FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario _scenario;

    public Given_FeitelijkeVerenigingWerdGeregistreerd_With_Minimal_Fields(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario;
    }

    [Fact]
    public async ValueTask Then_we_get_a_successful_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);
        response.Should().BeSuccessful();
    }

    [Fact]
    public async ValueTask Then_we_get_json_ld_as_content_type()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async ValueTask Then_we_get_a_detail_vereniging_response()
    {
        var responseMessage = await _publicApiClient.GetDetail(_scenario.VCode);

        var content = await responseMessage.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
