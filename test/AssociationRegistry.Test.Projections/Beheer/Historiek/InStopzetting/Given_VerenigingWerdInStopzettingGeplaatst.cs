namespace AssociationRegistry.Test.Projections.Beheer.Historiek.InStopzetting;

using Admin.Schema.Historiek;
using Events;
using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdInStopzettingGeplaatst(
    BeheerHistoriekScenarioFixture<VerenigingWerdInStopzettingGeplaatstScenario> fixture
) : BeheerHistoriekScenarioClassFixture<VerenigingWerdInStopzettingGeplaatstScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated() =>
        fixture
            .Result.Gebeurtenissen.Last()
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: "Vereniging werd in stopzetting geplaatst.",
                    nameof(VerenigingWerdInStopzettingGeplaatst),
                    fixture.Scenario.VerenigingWerdInStopzettingGeplaatst,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
