namespace AssociationRegistry.Test.Projections.Beheer.Historiek.InStopzetting;

using Admin.Schema.Historiek;
using Events;
using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitStopzettingGehaald(
    BeheerHistoriekScenarioFixture<VerenigingWerdUitStopzettingGehaaldScenario> fixture
) : BeheerHistoriekScenarioClassFixture<VerenigingWerdUitStopzettingGehaaldScenario>
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
                    Beschrijving: "Vereniging werd uit stopzetting gehaald.",
                    nameof(VerenigingWerdUitStopzettingGehaald),
                    fixture.Scenario.VerenigingWerdUitStopzettingGehaald,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
