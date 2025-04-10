namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Stopzetting.Vzer;

using Admin.Schema.Historiek;
using Events;
using Formats;
using Scenario.Stopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGestopt(
    BeheerHistoriekScenarioFixture<VerenigingWerdGestoptScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VerenigingWerdGestoptScenario>
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
                                               Beschrijving: $"De vereniging werd gestopt met einddatum '{fixture.Scenario.VerenigingWerdGestopt.Einddatum.ToString(WellknownFormats.DateOnly)}'.",
                                               nameof(VerenigingWerdGestopt),
                                               fixture.Scenario.VerenigingWerdGestopt,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
