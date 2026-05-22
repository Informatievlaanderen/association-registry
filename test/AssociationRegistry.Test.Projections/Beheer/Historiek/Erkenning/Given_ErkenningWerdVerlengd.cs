namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Erkenning;

using Admin.ProjectionHost.Constants;
using Admin.Schema.Historiek;
using Events;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerlengd(
    BeheerHistoriekScenarioFixture<ErkenningWerdVerlengdScenario> fixture
) : BeheerHistoriekScenarioClassFixture<ErkenningWerdVerlengdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated() =>
        fixture
            .Result.Gebeurtenissen.Last()
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: $"Erkenning werd verlengd naar {fixture.Scenario.ErkenningWerdVerlengd.Einddatum.ToString(WellknownFormats.DateOnly)}.",
                    nameof(ErkenningWerdVerlengd),
                    fixture.Scenario.ErkenningWerdVerlengd,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
