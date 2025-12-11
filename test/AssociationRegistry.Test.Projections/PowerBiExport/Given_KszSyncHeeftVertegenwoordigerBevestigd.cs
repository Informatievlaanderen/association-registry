namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Events;
using Scenario.Vertegenwoordigers.Kbo;
using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_KszSyncHeeftVertegenwoordigerBevestigd(PowerBiScenarioFixture<KszSyncHeeftVertegenwoordigerBevestigdScenario> fixture)
    : PowerBiScenarioClassFixture<KszSyncHeeftVertegenwoordigerBevestigdScenario>
{
    [Fact]
    public void Vertegenwoordigers_Count_Is_The_Same()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.AantalVertegenwoordigers.Should().Be(fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.Length);
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.Historiek.Should().NotBeEmpty();
        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(KszSyncHeeftVertegenwoordigerBevestigd));
    }
}
