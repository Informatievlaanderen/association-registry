namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using System.Threading.Tasks;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens
{
    private readonly V015_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens(GivenEventsFixture fixture)
    {
        _scenario = fixture.V015VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigBasisgegevens;

        _publicApiClient = fixture.PublicApiClient;

        _publicApiClient.GetDetail(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode).GetAwaiter()
                        .GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_detail_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);

        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                          .WithRoepnaam(_scenario.RoepnaamWerdGewijzigd.Roepnaam)
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
