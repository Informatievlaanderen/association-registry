namespace AssociationRegistry.Test.Projections.Beheer.Historiek.StartDatum.Vzer;

using Admin.Schema.Historiek;
using Events;
using Formats;
using Scenario.Startdatum.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_StartdatumWerdGewijzigd(
    BeheerHistoriekScenarioFixture<StartdatumWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<StartdatumWerdGewijzigdScenario>
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
                                               Beschrijving: $"Startdatum werd gewijzigd naar '{fixture.Scenario.StartdatumWerdGewijzigd.Startdatum.FormatAsBelgianDate()}'.",
                                               nameof(StartdatumWerdGewijzigd),
                                               fixture.Scenario.StartdatumWerdGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
