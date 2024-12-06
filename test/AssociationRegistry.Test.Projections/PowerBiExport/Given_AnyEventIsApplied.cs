namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Events;
using Publiek.Detail;

[Collection(nameof(ProjectionContext))]
public class Given_AnyEventIsApplied(PowerBiScenarioFixture<ApplyAllEventsScenario> fixture) : PowerBiScenarioClassFixture<ApplyAllEventsScenario>
{
    [Fact]
    public void AGebeurtenisIsAddedForEachEvent()
    {
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek[0].EventType
               .Should()
               .Be(fixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.GetType().Name);

        fixture.Result.Historiek[1].EventType.Should().Be(nameof(NaamWerdGewijzigd));
        fixture.Result.Historiek[2].EventType.Should().Be(nameof(LocatieWerdToegevoegd));
        fixture.Result.Historiek[3].EventType.Should().Be(nameof(LocatieWerdToegevoegd));
        fixture.Result.Historiek[4].EventType.Should().Be(nameof(VerenigingWerdGestopt));
    }
}
