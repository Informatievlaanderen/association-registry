namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Events;
using KellermanSoftware.CompareNetObjects;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdToegevoegdVanuitKBO(PowerBiScenarioFixture<BankrekeningnummerWerdToegevoegdVanuitKBOScenario> fixture)
    : PowerBiScenarioClassFixture<BankrekeningnummerWerdToegevoegdVanuitKBOScenario>
{
    [Fact]
    public void AantalBankrekeningnummers_Is_Increased_By_One()
    {
        fixture.Result.AantalBankrekeningnummers.Should().Be(1);
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(BankrekeningnummerWerdToegevoegdVanuitKBO));
    }
}
