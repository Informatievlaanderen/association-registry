namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Vzer;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdGevalideerd(
    BeheerHistoriekScenarioFixture<BankrekeningnummerWerdGevalideerdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<BankrekeningnummerWerdGevalideerdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: $"Bankrekeningnummer validatie document werd bevestigd door '{fixture.Scenario.AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd.BevestigdDoor}'.",
                                               nameof(AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd),
                                               fixture.Scenario.AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
