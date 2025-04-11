namespace AssociationRegistry.Test.Projections.Beheer.Historiek.StartDatum.Kbo;

using Admin.Schema.Historiek;
using Events;
using Formats;
using Scenario.Startdatum.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_StartdatumWerdGewijzigdInKbo(
    BeheerHistoriekScenarioFixture<StartdatumWerdGewijzigdInKboScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<StartdatumWerdGewijzigdInKboScenario>
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
                                               Beschrijving: $"In KBO werd de startdatum gewijzigd naar '{fixture.Scenario.StartdatumWerdGewijzigdInKbo.Startdatum.FormatAsBelgianDate()}'.",
                                               nameof(StartdatumWerdGewijzigdInKbo),
                                               fixture.Scenario.StartdatumWerdGewijzigdInKbo,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
