namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Koepels;

using Admin.Schema.Historiek;
using Events;
using Scenario.Koepels;

[Collection(nameof(ProjectionContext))]
public class Given_KoepelstatusWerdTerugGezetNaarNietBepaald(
    BeheerHistoriekScenarioFixture<KoepelstatusWerdTerugGezetNaarNietBepaaldScenario> fixture
) : BeheerHistoriekScenarioClassFixture<KoepelstatusWerdTerugGezetNaarNietBepaaldScenario>
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
                    Beschrijving: "Koepelstatus werd teruggezet naar niet bepaald.",
                    nameof(KoepelstatusWerdTerugGezetNaarNietBepaald),
                    fixture.Scenario.KoepelstatusWerdTerugGezetNaarNietBepaald,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
