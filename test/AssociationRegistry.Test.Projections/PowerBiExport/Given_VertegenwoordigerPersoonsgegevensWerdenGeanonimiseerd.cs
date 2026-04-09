namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Events;
using Scenario.VertegenwoordigerPersoonsgegevens;
using Scenario.Vertegenwoordigers.Kbo;
using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(
    PowerBiScenarioFixture<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdScenario> fixture
) : PowerBiScenarioClassFixture<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdScenario>
{
    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.Historiek.Should().NotBeEmpty();
        fixture
            .Result.Historiek.Should()
            .ContainSingle(x => x.EventType == nameof(VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd));
    }
}
