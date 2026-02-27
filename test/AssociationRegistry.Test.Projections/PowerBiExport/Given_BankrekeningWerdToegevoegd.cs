namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Events;
using Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdToegevoegd(PowerBiScenarioFixture<BankrekeningnummerWerdToegevoegdScenario> fixture)
    : PowerBiScenarioClassFixture<BankrekeningnummerWerdToegevoegdScenario>
{
    [Fact]
    public void Bankrekeningnummer_Is_Added()
    {
        fixture.Result.Bankrekeningnummers.Should().ContainEquivalentOf(new Bankrekeningnummer(
                                                        fixture.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                                                        fixture.Scenario.BankrekeningnummerWerdToegevoegd.Doel,
                                                        [],
                                                        fixture.Scenario.BankrekeningnummerWerdToegevoegd.Bron
                                                        ));
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
