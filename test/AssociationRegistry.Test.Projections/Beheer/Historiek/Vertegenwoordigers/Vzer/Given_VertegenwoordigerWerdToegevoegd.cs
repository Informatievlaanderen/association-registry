namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Vertegenwoordigers.Vzer;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdToegevoegd(
    BeheerHistoriekScenarioFixture<VertegenwoordigerWerdToegevoegdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerWerdToegevoegdScenario>
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
                                               Beschrijving:$"'{fixture.Scenario.VertegenwoordigerWerdToegevoegd.Voornaam} {fixture.Scenario.VertegenwoordigerWerdToegevoegd.Achternaam}' werd toegevoegd als vertegenwoordiger.",
                                               nameof(VertegenwoordigerWerdToegevoegd),
                                               VertegenwoordigerData.Create(fixture.Scenario.VertegenwoordigerWerdToegevoegd),
                                                                            fixture.MetadataInitiator,
                                                                            fixture.MetadataTijdstip));
}
