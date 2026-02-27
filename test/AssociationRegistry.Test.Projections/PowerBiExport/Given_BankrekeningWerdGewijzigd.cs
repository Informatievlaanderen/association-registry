namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Events;
using Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdGewijzigd(PowerBiScenarioFixture<BankrekeningnummerWerdGewijzigdScenario> fixture)
    : PowerBiScenarioClassFixture<BankrekeningnummerWerdGewijzigdScenario>
{
    [Fact]
    public void Bankrekeningnummer_Is_Changed()
    {
        fixture.Result.Bankrekeningnummers.Should().ContainEquivalentOf(new Bankrekeningnummer(fixture.Scenario.BankrekeningnummerWerdGewijzigd.BankrekeningnummerId,
                                                                fixture.Scenario.BankrekeningnummerWerdGewijzigd.Doel,
                                                                [],
                                                                fixture.Scenario.BankrekeningnummerWerdGewijzigd.Bron));
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(BankrekeningnummerWerdGewijzigd));
    }
}
