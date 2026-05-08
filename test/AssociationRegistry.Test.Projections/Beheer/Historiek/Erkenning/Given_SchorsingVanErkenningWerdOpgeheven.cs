namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Erkenning;

using Admin.Schema.Historiek;
using Events;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_SchorsingVanErkenningWerdOpgeheven(
    BeheerHistoriekScenarioFixture<SchorsingVanErkenningWerdOpgehevenScenario> fixture
) : BeheerHistoriekScenarioClassFixture<SchorsingVanErkenningWerdOpgehevenScenario>
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
                    Beschrijving: "Schorsing van erkenning werd opgeheven",
                    nameof(SchorsingVanErkenningWerdOpgeheven),
                    fixture.Scenario.SchorsingVanErkenningWerdOpgeheven,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
