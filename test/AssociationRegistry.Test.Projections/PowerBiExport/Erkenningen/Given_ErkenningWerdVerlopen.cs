namespace AssociationRegistry.Test.Projections.PowerBiExport.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Events;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerlopen(PowerBiScenarioFixture<ErkenningWerdVerlopenScenario> fixture)
    : PowerBiScenarioClassFixture<ErkenningWerdVerlopenScenario>
{
    [Fact]
    public void Erkenning_Werd_Verlopen()
    {
        fixture.Result.Erkenningen.Single().Status.Should().Be(ErkenningStatus.Verlopen.Value);
    }

    [Fact]
    public void Historiek_Is_Updated()
    {
        fixture.Result.Historiek.Last().EventType.Should().Be(nameof(ErkenningWerdVerlopen));
    }
}
