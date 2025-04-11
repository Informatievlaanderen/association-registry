namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Einddatum;

using Admin.Schema.Historiek;
using AssociationRegistry.Acm.Api.Constants;
using Events;
using Scenario.Einddatum;

[Collection(nameof(ProjectionContext))]
public class Given_EinddatumWerdGewijzigd(
    BeheerHistoriekScenarioFixture<EinddatumWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<EinddatumWerdGewijzigdScenario>
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
                                               Beschrijving: $"De einddatum van de vereniging werd gewijzigd naar '{fixture.Scenario.EinddatumWerdGewijzigd.Einddatum.ToString(WellknownFormats.DateOnly)}'.",
                                               nameof(EinddatumWerdGewijzigd),
                                               fixture.Scenario.EinddatumWerdGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
