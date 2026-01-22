namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Events;
using KellermanSoftware.CompareNetObjects;
using Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdVerwijderd(PowerBiScenarioFixture<BankrekeningnummerWerdVerwijderdScenario> fixture)
    : PowerBiScenarioClassFixture<BankrekeningnummerWerdVerwijderdScenario>
{
    [Fact]
    public void AantalBankrekeningnummers_Is_Increased_By_One()
    {
        fixture.Result.AantalBankrekeningnummers.Should().Be(0);
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(BankrekeningnummerWerdVerwijderd));
    }
}
