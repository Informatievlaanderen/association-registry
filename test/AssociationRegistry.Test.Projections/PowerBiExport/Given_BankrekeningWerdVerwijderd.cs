namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Events;
using Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdVerwijderd(PowerBiScenarioFixture<BankrekeningnummerWerdVerwijderdScenario> fixture)
    : PowerBiScenarioClassFixture<BankrekeningnummerWerdVerwijderdScenario>
{
    [Fact]
    public void Bankrekeningnummer_Should_Be_Removed()
    {
        fixture.Result.Bankrekeningnummers
               .FirstOrDefault(b => b.BankrekeningnummerId ==
                                    fixture.Scenario.BankrekeningnummerWerdVerwijderd.BankrekeningnummerId)
               .Should().BeNull();
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should()
               .Be(fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);

        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek.Should()
               .ContainSingle(x => x.EventType == nameof(BankrekeningnummerWerdVerwijderd));
    }
}
