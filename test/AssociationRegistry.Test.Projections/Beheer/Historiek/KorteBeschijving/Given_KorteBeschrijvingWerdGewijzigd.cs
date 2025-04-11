namespace AssociationRegistry.Test.Projections.Beheer.Historiek.KorteBeschijving;

using Admin.Schema.Historiek;
using AssociationRegistry.Test.Projections.Scenario.KorteBeschijving;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_KorteBeschrijvingWerdGewijzigd(
    BeheerHistoriekScenarioFixture<KorteBeschrijvingWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<KorteBeschrijvingWerdGewijzigdScenario>
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
                                               Beschrijving: $"Korte beschrijving werd gewijzigd naar '{fixture.Scenario.KorteBeschrijvingWerdGewijzigd.KorteBeschrijving}'.",
                                               nameof(KorteBeschrijvingWerdGewijzigd),
                                               fixture.Scenario.KorteBeschrijvingWerdGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
