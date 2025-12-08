namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Events;
using Scenario.Vertegenwoordigers.Kbo;
using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend(PowerBiScenarioFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendScenario> fixture)
    : PowerBiScenarioClassFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendScenario>
{
    [Fact]
    public void Vertegenwoordigers_Count_Is_Decreased()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.AantalVertegenwoordigers.Should().Be(fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.Length - 1);
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.Historiek.Should().NotBeEmpty();
        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend));
    }
}
