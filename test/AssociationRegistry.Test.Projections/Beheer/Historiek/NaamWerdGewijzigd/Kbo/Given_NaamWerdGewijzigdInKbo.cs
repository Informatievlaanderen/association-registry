namespace AssociationRegistry.Test.Projections.Beheer.Historiek.NaamWerdGewijzigd.Kbo;

using Admin.Schema.Historiek;
using AssociationRegistry.Test.Projections.Scenario.NaamWerdGewijzigd.Kbo;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_NaamWerdGewijzigdInKbo(
    BeheerHistoriekScenarioFixture<NaamWerdGewijzigdInKboScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<NaamWerdGewijzigdInKboScenario>
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
                                               Beschrijving: $"In KBO werd de naam gewijzigd naar '{@fixture.Scenario.NaamWerdGewijzigdInKbo.Naam}'.",
                                               nameof(NaamWerdGewijzigdInKbo),
                                               fixture.Scenario.NaamWerdGewijzigdInKbo,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
