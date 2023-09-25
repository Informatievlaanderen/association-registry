namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

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
public class Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields
{
    private readonly V009_MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario _scenario;
    private PublicApiClient _publicApiClient;

    public Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields(GivenEventsFixture fixture)
    {
        _scenario = fixture.V009MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_detail_afdeling_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);

        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.AfdelingWerdGeregistreerd)
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task Then_we_get_a_detail_moeder_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.MoederWerdGeregistreerd.VCode);

        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.MoederWerdGeregistreerd)
                          .HeeftAfdeling(_scenario.AfdelingWerdGeregistreerd.VCode, _scenario.AfdelingWerdGeregistreerd.Naam)
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
