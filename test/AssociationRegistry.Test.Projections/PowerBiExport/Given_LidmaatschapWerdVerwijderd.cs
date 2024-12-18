namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Events;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdVerwijderd(PowerBiScenarioFixture<LidmaatschapWerdVerwijderdScenario> fixture)
    : PowerBiScenarioClassFixture<LidmaatschapWerdVerwijderdScenario>
{
    [Fact]
    public void ARecordIsStored_With_Lidmaatschap()
    {
        fixture.Result.Lidmaatschappen.Should().BeEmpty();
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.LidmaatschapWerdVerwijderd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(LidmaatschapWerdVerwijderd));
    }
}
