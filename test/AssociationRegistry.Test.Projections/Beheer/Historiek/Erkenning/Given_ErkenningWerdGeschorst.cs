namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Erkenning;

using Admin.Schema.Historiek;
using Events;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGeschorst(
    BeheerHistoriekScenarioFixture<ErkenningWerdGeschorstScenario> fixture
) : BeheerHistoriekScenarioClassFixture<ErkenningWerdGeschorstScenario>
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
                    Beschrijving: $"Erkenning werd geschorst om deze reden: {fixture.Scenario.ErkenningWerdGeschorst.RedenSchorsing}.",
                    nameof(ErkenningWerdGeschorst),
                    fixture.Scenario.ErkenningWerdGeschorst,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
