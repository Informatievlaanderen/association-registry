namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Bankrekeningen.Vzer;

using Admin.Schema.Historiek;
using Events;
using Scenario.Bankrekeningnummers.Vzer;

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
    public void Historiek_Saved_Validatie_Bankrekenummer_Werd_Ongedaan_Gemaakt()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving:
                                               $"Bankrekeningnummervalidatiedocument werd ongedaan gemaakt door '{fixture.Scenario.AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt.OngedaanGemaaktDoor}'.",
                                               nameof(
                                                   AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt),
                                               fixture.Scenario
                                                      .AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
