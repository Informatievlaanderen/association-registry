namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.KboStatusCorrectie;

using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Test.Projections.Scenario.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_KBOStatusWerdGecorrigeerdNaarActief(
    PubliekZoekenScenarioFixture<KBOStatusWerdGecorrigeerdNaarActiefScenario> fixture
) : PubliekZoekenScenarioClassFixture<KBOStatusWerdGecorrigeerdNaarActiefScenario>
{
    [Fact]
    public void Status_Is_Actief()
    {
        fixture.Result.Status.Should().Be(VerenigingStatus.Actief);
    }
}
