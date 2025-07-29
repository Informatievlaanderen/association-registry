namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Lidmaatschappen;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdToegevoegd(BeheerHistoriekScenarioFixture<LidmaatschapWerdToegevoegdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<LidmaatschapWerdToegevoegdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Should().ContainEquivalentOf(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: "Lidmaatschap werd toegevoegd.",
                                               nameof(LidmaatschapWerdToegevoegd),
                                               LidmaatschapData.Create(fixture.Scenario.LidmaatschapWerdToegevoegdFirst.Lidmaatschap),
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
