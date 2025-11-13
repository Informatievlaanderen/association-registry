namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Vertegenwoordigers.Vzer;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdVerwijderd(
    BeheerHistoriekScenarioFixture<VertegenwoordigerWerdVerwijderdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerWerdVerwijderdScenario>
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
                                               Beschrijving:$"Vertegenwoordiger '{fixture.Scenario.VertegenwoordigerWerdVerwijderdMetPersoonsgegevens.Voornaam} {fixture.Scenario.VertegenwoordigerWerdVerwijderdMetPersoonsgegevens.Achternaam}' werd verwijderd.",
                                               nameof(VertegenwoordigerWerdVerwijderd),
                                               VertegenwoordigerWerdVerwijderdData.Create(fixture.Scenario.VertegenwoordigerWerdVerwijderdMetPersoonsgegevens),
                                                                            fixture.MetadataInitiator,
                                                                            fixture.MetadataTijdstip));
}
