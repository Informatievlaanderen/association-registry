namespace AssociationRegistry.Test.Projections.PowerBiExport.Kbo;

using DecentraalBeheer.Vereniging;
using Events;
using Scenario.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_KBOStatusWerdGecorrigeerdNaarActief(
    PowerBiScenarioFixture<KBOStatusWerdGecorrigeerdNaarActiefScenario> fixture
) : PowerBiScenarioClassFixture<KBOStatusWerdGecorrigeerdNaarActiefScenario>
{
    [Fact]
    public void Status_Is_Set_To_Actief()
    {
        fixture.Result.Status.Should().Be(VerenigingStatus.Actief.StatusNaam);
    }

    [Fact]
    public void Einddatum_Is_Null()
    {
        fixture.Result.Einddatum.Should().BeNull();
    }

    [Fact]
    public void Event_KBOStatusWerdGecorrigeerdNaarActief_Is_Added_In_Historiek()
    {
        fixture
            .Result.Historiek.Should()
            .ContainSingle(x => x.EventType == nameof(KBOStatusWerdGecorrigeerdNaarActief));
    }
}
