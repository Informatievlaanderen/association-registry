namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Persoonsgegevens.Vertegenwoordigers;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerPersoonsGegevensWerdenGeanonimiseerd(
    BeheerHistoriekScenarioFixture<VertegenwoordigerWerdGewijzigdInKBOScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerWerdGewijzigdInKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(1);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving:$"Vertegenwoordiger '{fixture.Scenario.VertegenwoordigerWerdGewijzigdInKBO.Voornaam} {fixture.Scenario.VertegenwoordigerWerdGewijzigdInKBO.Achternaam}' werd gewijzigd in KBO.",
                                               nameof(VertegenwoordigerWerdGewijzigdInKBO),
                                               KBOVertegenwoordigerData.Create(fixture.Scenario.VertegenwoordigerWerdGewijzigdInKBO),
                                                                            fixture.MetadataInitiator,
                                                                            fixture.MetadataTijdstip));
}
