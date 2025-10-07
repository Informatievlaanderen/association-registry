namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Vertegenwoordigers.Kbo;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.Vertegenwoordigers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdVerwijderdUitKBO(
    BeheerHistoriekScenarioFixture<VertegenwoordigerWerdVerwijderdUitKBOScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerWerdVerwijderdUitKBOScenario>
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
                                               Beschrijving:$"Vertegenwoordiger '{fixture.Scenario.VertegenwoordigerWerdVerwijderdUitKBO.Voornaam} {fixture.Scenario.VertegenwoordigerWerdVerwijderdUitKBO.Achternaam}' werd verwijderd uit KBO.",
                                               nameof(VertegenwoordigerWerdVerwijderdUitKBO),
                                               VertegenwoordigerData.Create(fixture.Scenario.VertegenwoordigerWerdVerwijderdUitKBO),
                                                                            fixture.MetadataInitiator,
                                                                            fixture.MetadataTijdstip));
}
