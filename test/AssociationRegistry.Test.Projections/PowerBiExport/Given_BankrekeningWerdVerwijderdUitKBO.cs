namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Events;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdVerwijderdUitKBO(PowerBiScenarioFixture<BankrekeningnummerWerdVerwijderdUitKBOScenario> fixture)
    : PowerBiScenarioClassFixture<BankrekeningnummerWerdVerwijderdUitKBOScenario>
{
    [Fact]
    public void Bankrekeningnummers_Are_Empty()
    {
        fixture.Result.Bankrekeningnummers.Should().BeEmpty();
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(BankrekeningnummerWerdVerwijderdUitKBO));
    }
}
