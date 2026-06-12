namespace AssociationRegistry.Test.Projections.Beheer.Historiek.KorteNaam.Kbo;

using Admin.Schema.Historiek;
using Events;
using Scenario.KorteNaamWerdGewijzigd.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_KorteNaamWerdGewijzigdInKbo(
    BeheerHistoriekScenarioFixture<KorteNaamWerdGewijzigdInKboScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<KorteNaamWerdGewijzigdInKboScenario>
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
                                               $"In KBO werd de korte naam gewijzigd naar '{fixture.Scenario.KorteNaamWerdGewijzigdInKbo.KorteNaam}'.",
                                               nameof(KorteNaamWerdGewijzigdInKbo),
                                               fixture.Scenario.KorteNaamWerdGewijzigdInKbo,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
