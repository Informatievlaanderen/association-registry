namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Vertegenwoordigers.Vzer;

using Admin.Schema.Historiek;
using AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdGewijzigd(
    BeheerHistoriekScenarioFixture<VertegenwoordigerWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving:$"Vertegenwoordiger '{fixture.Scenario.VertegenwoordigerWerdGewijzigd.Voornaam} {fixture.Scenario.VertegenwoordigerWerdGewijzigd.Achternaam}' werd gewijzigd.",
                                               nameof(VertegenwoordigerWerdGewijzigd),
                                               fixture.Scenario.VertegenwoordigerWerdGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
