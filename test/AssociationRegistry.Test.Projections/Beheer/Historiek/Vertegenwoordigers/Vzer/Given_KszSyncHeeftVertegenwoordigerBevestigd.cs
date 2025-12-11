namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Vertegenwoordigers.Vzer;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_KszSyncHeeftVertegenwoordigerBevestigd(
    BeheerHistoriekScenarioFixture<KszSyncHeeftVertegenwoordigerBevestigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<KszSyncHeeftVertegenwoordigerBevestigdScenario>
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
                                               Beschrijving:$"Vertegenwoordiger met id: '{fixture.Scenario.KszSyncHeeftVertegenwoordigerBevestigd.VertegenwoordigerId}' is bevestigd door KSZ.",
                                               nameof(KszSyncHeeftVertegenwoordigerBevestigd),
                                               Data: new object(),
                                                                            fixture.MetadataInitiator,
                                                                            fixture.MetadataTijdstip));
}
