namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Erkenning;

using Admin.Schema.Historiek;
using Events;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerlopen(BeheerHistoriekScenarioFixture<ErkenningWerdVerlopenScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<ErkenningWerdVerlopenScenario>
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
                    Beschrijving: "Erkenning werd verlopen",
                    nameof(ErkenningWerdVerlopen),
                    fixture.Scenario.ErkenningWerdVerlopen,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
