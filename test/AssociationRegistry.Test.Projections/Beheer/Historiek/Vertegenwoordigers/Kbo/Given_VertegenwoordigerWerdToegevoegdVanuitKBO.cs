namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Vertegenwoordigers.Kbo;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.Vertegenwoordigers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdToegevoegdVanuitKBO(
    BeheerHistoriekScenarioFixture<VertegenwoordigerWerdToegevoegdVanuitKBOScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerWerdToegevoegdVanuitKBOScenario>
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
                                               Beschrijving:$"Vertegenwoordiger '{fixture.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO.Voornaam} {fixture.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO.Achternaam}' werd toegevoegd vanuit KBO.",
                                               nameof(VertegenwoordigerWerdToegevoegdVanuitKBO),
                                               KBOVertegenwoordigerData.Create(fixture.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO),
                                                                            fixture.MetadataInitiator,
                                                                            fixture.MetadataTijdstip));
}
