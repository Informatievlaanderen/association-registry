namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Erkenning;

using Admin.Schema.Historiek;
using Events;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningRedenVanSchorsingWerdGecorrigeerd(
    BeheerHistoriekScenarioFixture<ErkenningRedenVanSchorsingWerdGecorrigeerdScenario> fixture
) : BeheerHistoriekScenarioClassFixture<ErkenningRedenVanSchorsingWerdGecorrigeerdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(4);

    [Fact]
    public void Document_Is_Updated() =>
        fixture
            .Result.Gebeurtenissen.Last()
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: $"Reden van schorsing van een erkenning werd gecorrigeerd: {fixture.Scenario.ErkenningRedenVanSchorsingWerdGecorrigeerd.RedenSchorsing}",
                    nameof(ErkenningRedenVanSchorsingWerdGecorrigeerd),
                    fixture.Scenario.ErkenningRedenVanSchorsingWerdGecorrigeerd,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
