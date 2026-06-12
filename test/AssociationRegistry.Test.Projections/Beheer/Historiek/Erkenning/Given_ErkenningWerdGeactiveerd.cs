namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Erkenning;

using Admin.Schema.Historiek;
using Events;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGeactiveerd(BeheerHistoriekScenarioFixture<ErkenningWerdGeactiveerdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<ErkenningWerdGeactiveerdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Historiek_Saved_Erkenning_Werd_Geactiveerd() =>
        fixture
           .Result.Gebeurtenissen.Last()
           .Should()
           .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: "Erkenning werd geactiveerd",
                    nameof(ErkenningWerdGeactiveerd),
                    fixture.Scenario.ErkenningWerdGeactiveerd,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
