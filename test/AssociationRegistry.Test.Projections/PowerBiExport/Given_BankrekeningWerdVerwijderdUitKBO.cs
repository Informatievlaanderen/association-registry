namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Events;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdVerwijderdUitKBO(
    PowerBiScenarioFixture<BankrekeningnummerWerdVerwijderdUitKBOScenario> fixture
) : PowerBiScenarioClassFixture<BankrekeningnummerWerdVerwijderdUitKBOScenario>
{
    [Fact]
    public void Bankrekeningnummer_Should_Be_Removed()
    {
        fixture.Result.Bankrekeningnummers
               .FirstOrDefault(b => b.BankrekeningnummerId ==
                                    fixture.Scenario.BankrekeningnummerWerdVerwijderdUitKBO.BankrekeningnummerId)
               .Should().BeNull();

    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture
            .Result.Historiek.Should()
            .ContainSingle(x => x.EventType == nameof(BankrekeningnummerWerdVerwijderdUitKBO));
    }
}
