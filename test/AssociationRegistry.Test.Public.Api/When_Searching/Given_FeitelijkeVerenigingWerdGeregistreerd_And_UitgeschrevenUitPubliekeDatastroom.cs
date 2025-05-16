namespace AssociationRegistry.Test.Public.Api.When_Searching;

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
public class Given_FeitelijkeVerenigingWerdGeregistreerd_And_UitgeschrevenUitPubliekeDatastroom
{
    private readonly V010_FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_FeitelijkeVerenigingWerdGeregistreerd_And_UitgeschrevenUitPubliekeDatastroom(GivenEventsFixture fixture)
    {
        _scenario = fixture.V010FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_no_vereniging_is_retrieved_by_its_vCode()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
    }
}
