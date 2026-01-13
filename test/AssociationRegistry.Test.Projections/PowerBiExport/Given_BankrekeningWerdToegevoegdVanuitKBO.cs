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
    public void ARecordIsStored_With_Bankrekeningen()
    {
        Bankrekeningnummer[] expectedBankrekeningnummers =
        [
            new(
                fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId,
                fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.Iban,
                string.Empty,
                string.Empty
            ),
        ];

        fixture.Result.Bankrekeningnummers.ShouldCompare(expectedBankrekeningnummers);
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
