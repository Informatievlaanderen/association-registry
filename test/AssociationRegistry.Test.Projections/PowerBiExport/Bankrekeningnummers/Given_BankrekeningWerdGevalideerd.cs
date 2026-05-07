namespace AssociationRegistry.Test.Projections.PowerBiExport.Bankrekeningnummers;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdGevalideerd(PowerBiScenarioFixture<BankrekeningnummerWerdGevalideerdScenario> fixture)
    : PowerBiScenarioClassFixture<BankrekeningnummerWerdGevalideerdScenario>
{
    [Fact]
    public void Bankrekeningnummer_Should_Be_Bevestigd()
    {
        fixture.Result.Bankrekeningnummers.Should().ContainEquivalentOf(new Bankrekeningnummer(
                                                                            fixture.Scenario.AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd.BankrekeningnummerId,
                                                                            fixture.Scenario.BankrekeningnummerWerdToegevoegd.Doel,
                                                                            [
                                                                                fixture.Scenario
                                                                                   .AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd
                                                                                   .BevestigdDoor,
                                                                            ],
                                                                            fixture.Scenario.AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd.Bron
                                                                            ));
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd));
    }
}
