namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Vertegenwoordigers.Vzer;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.Vertegenwoordigers.Vzer;

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
    public void Historiek_Saved_BeheerVerenigingHistoriekGebeurtenis()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving:
                                               $"Vertegenwoordiger '{fixture.Scenario.VertegenwoordigerWerdVerwijderd.Voornaam} {fixture.Scenario.VertegenwoordigerWerdVerwijderd.Achternaam}' werd verwijderd.",
                                               nameof(VertegenwoordigerWerdVerwijderd),
                                               VertegenwoordigerWerdVerwijderdData.Create(
                                                   fixture.Scenario.VertegenwoordigerWerdVerwijderd),
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
