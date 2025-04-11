namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Stopzetting.Kbo;

using Admin.Schema.Historiek;
using Events;
using Formats;
using Scenario.Stopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGestoptInKbo(
    BeheerHistoriekScenarioFixture<VerenigingWerdGestoptInKBOScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VerenigingWerdGestoptInKBOScenario>
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
                                               Beschrijving: $"De vereniging werd gestopt in KBO met einddatum '{fixture.Scenario.VerenigingWerdGestoptInKBO.Einddatum.ToString(WellknownFormats.DateOnly)}'.",
                                               nameof(VerenigingWerdGestoptInKBO),
                                               fixture.Scenario.VerenigingWerdGestoptInKBO,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
