namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Vzer;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_ValidatieBankrekeningWerdOngedaanGemaakt(
    BeheerHistoriekScenarioFixture<ValidatieBankrekeningnummerWerdOngedaanGemaaktScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<ValidatieBankrekeningnummerWerdOngedaanGemaaktScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(4);

    [Fact]
    public void Then_Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: $"Bankrekeningnummervalidatiedocument werd ongedaan gemaakt door '{fixture.Scenario.AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt.OngedaanGemaaktDoor}'.",
                                               nameof(AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt),
                                               fixture.Scenario.AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
