namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using templates;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class Given_WerdUitgeschrevenUitPubliekeDatastroom_And_NaamGewijzigd
{
    private readonly V023_WerdUitgeschrevenUitPubliekeDatastroomScenario_And_NaamWerdGewijzigd _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_WerdUitgeschrevenUitPubliekeDatastroom_And_NaamGewijzigd(GivenEventsFixture fixture)
    {
        _scenario = fixture.V023WerdUitgeschrevenUitPubliekeDatastroomScenarioAndNaamWerdGewijzigd;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async ValueTask Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async ValueTask Then_no_vereniging_is_retrieved_by_its_vCode()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
    }
}
