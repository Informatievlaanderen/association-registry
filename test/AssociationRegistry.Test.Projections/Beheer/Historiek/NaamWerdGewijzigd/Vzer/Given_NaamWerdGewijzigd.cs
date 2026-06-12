namespace AssociationRegistry.Test.Projections.Beheer.Historiek.NaamWerdGewijzigd.Vzer;

using Admin.Schema.Historiek;
using Events;
using Scenario.NaamWerdGewijzigd.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_NaamWerdGewijzigd(
    BeheerHistoriekScenarioFixture<NaamWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<NaamWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Historiek_Saved_Naam_Werd_Gewijzigd()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving:
                                               $"Naam werd gewijzigd naar '{fixture.Scenario.NaamWerdGewijzigd.Naam}'.",
                                               nameof(NaamWerdGewijzigd),
                                               fixture.Scenario.NaamWerdGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
