namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Events;
using KellermanSoftware.CompareNetObjects;
using Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdToegevoegd(PowerBiScenarioFixture<BankrekeningnummerWerdToegevoegdScenario> fixture)
    : PowerBiScenarioClassFixture<BankrekeningnummerWerdToegevoegdScenario>
{
    [Fact]
    public void ARecordIsStored_With_Bankrekeningen()
    {
        Bankrekeningnummer[] expectedLidmaatschap =
        [
            new(
                fixture.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                fixture.Scenario.BankrekeningnummerWerdToegevoegd.Iban,
                fixture.Scenario.BankrekeningnummerWerdToegevoegd.Doel,
                fixture.Scenario.BankrekeningnummerWerdToegevoegd.Titularis
            ),
        ];

        fixture.Result.Bankrekeningnummers.ShouldCompare(expectedLidmaatschap);
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(BankrekeningnummerWerdToegevoegd));
    }
}
