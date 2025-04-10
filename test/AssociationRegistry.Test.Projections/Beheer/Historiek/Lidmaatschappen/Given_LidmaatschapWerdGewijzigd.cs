namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Lidmaatschappen;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdGewijzigd(BeheerHistoriekScenarioFixture<LidmaatschapWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<LidmaatschapWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: "Lidmaatschap werd gewijzigd.",
                                               nameof(LidmaatschapWerdGewijzigd),
                                               LidmaatschapData.Create(fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap),
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
