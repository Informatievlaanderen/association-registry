namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Erkenning;

using Admin.Schema.Historiek;
using Events;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGeregistreerd(
    BeheerHistoriekScenarioFixture<ErkenningWerdGeregistreerdScenario> fixture
) : BeheerHistoriekScenarioClassFixture<ErkenningWerdGeregistreerdScenario>
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
                    Beschrijving: $"Erkenning werd geregistreerd door '{fixture.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.Naam}'.",
                    nameof(ErkenningWerdGeregistreerd),
                    fixture.Scenario.ErkenningWerdGeregistreerd,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
