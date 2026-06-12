namespace AssociationRegistry.Test.Projections.Beheer.Historiek.KorteNaam.Vzer;

using Admin.Schema.Historiek;
using Events;
using Scenario.KorteNaamWerdGewijzigd.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_KorteNaamWerdGewijzigd(
    BeheerHistoriekScenarioFixture<KorteNaamWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<KorteNaamWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Historiek_Saved_Korte_Naam_Werd_Gewijzigd()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving:
                                               $"Korte naam werd gewijzigd naar '{@fixture.Scenario.KorteNaamWerdGewijzigd.KorteNaam}'.",
                                               nameof(KorteNaamWerdGewijzigd),
                                               fixture.Scenario.KorteNaamWerdGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
