namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.KboStatusCorrectie;

using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Test.Projections.Scenario.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_KBOStatusWerdGecorrigeerdNaarActief(
    BeheerZoekenScenarioFixture<KBOStatusWerdGecorrigeerdNaarActiefScenario> fixture
) : BeheerZoekenScenarioClassFixture<KBOStatusWerdGecorrigeerdNaarActiefScenario>
{
    [Fact]
    public void Status_Is_Actief()
    {
        fixture.Result.Status.Should().Be(VerenigingStatus.Actief);
    }

    [Fact]
    public void Einddatum_Is_Null()
    {
        fixture.Result.Einddatum.Should().BeNull();
    }
}
