namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Events;

[Collection(nameof(ProjectionContext))]
public class Given_AnyEventIsApplied(ApplyAllEventsFixture fixture) : IClassFixture<ApplyAllEventsFixture>
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
