namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Vertegenwoordigers.Kbo;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.Vertegenwoordigers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdOvergenomenUitKBO(
    BeheerHistoriekScenarioFixture<VertegenwoordigerWerdOvergenomenUitKBOScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerWerdOvergenomenUitKBOScenario>
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
                                               Beschrijving:$"Vertegenwoordiger '{fixture.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.Voornaam} {fixture.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.Achternaam}' werd overgenomen uit KBO.",
                                               nameof(VertegenwoordigerWerdOvergenomenUitKBO),
                                               VertegenwoordigerData.Create(fixture.Scenario.VertegenwoordigerWerdOvergenomenUitKBO),
                                                                            fixture.MetadataInitiator,
                                                                            fixture.MetadataTijdstip));
}
