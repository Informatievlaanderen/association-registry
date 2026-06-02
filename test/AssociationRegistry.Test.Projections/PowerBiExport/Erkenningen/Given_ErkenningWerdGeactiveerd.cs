namespace AssociationRegistry.Test.Projections.PowerBiExport.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Events;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGeactiveerd(PowerBiScenarioFixture<ErkenningWerdGeactiveerdScenario> fixture)
    : PowerBiScenarioClassFixture<ErkenningWerdGeactiveerdScenario>
{
    [Fact]
    public void Erkenning_Werd_Geactiveerd()
    {
        fixture.Result.Erkenningen.Single().Status.Should().Be(ErkenningStatus.Actief.Value);
    }

    [Fact]
    public void Historiek_Is_Updated()
    {
        fixture.Result.Historiek.Last().EventType.Should().Be(nameof(ErkenningWerdGeactiveerd));
    }
}
