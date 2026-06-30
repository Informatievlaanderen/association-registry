namespace AssociationRegistry.Test.Projections.Publiek.Detail.KboStatusCorrectie;

using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Test.Projections.Scenario.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_KBOStatusWerdGecorrigeerdNaarActief(
    PubliekDetailScenarioFixture<KBOStatusWerdGecorrigeerdNaarActiefScenario> fixture
) : PubliekDetailScenarioClassFixture<KBOStatusWerdGecorrigeerdNaarActiefScenario>
{
    [Fact]
    public void Status_Is_Actief()
    {
        fixture.Result.Status.Should().Be(VerenigingStatus.Actief);
    }
}
