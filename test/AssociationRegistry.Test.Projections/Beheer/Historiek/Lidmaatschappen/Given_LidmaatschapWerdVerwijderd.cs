namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Lidmaatschappen;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdVerwijderd(
    BeheerHistoriekScenarioFixture<LidmaatschapWerdVerwijderdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<LidmaatschapWerdVerwijderdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(4);

    [Fact]
    public void Historiek_Saved_Lidmaatschap_Werd_Verwijderd()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: "Lidmaatschap werd verwijderd.",
                                               nameof(LidmaatschapWerdVerwijderd),
                                               LidmaatschapData.Create(
                                                   fixture.Scenario.LidmaatschapWerdVerwijderd.Lidmaatschap),
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
