namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Werkingsgebieden;

using Admin.Schema.Historiek;
using Events;
using Scenario.Werkingsgebieden;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietBepaald(BeheerHistoriekScenarioFixture<WerkingsgebiedenWerdenNietBepaaldScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should()
                  .BeEquivalentTo(
                       new BeheerVerenigingHistoriekGebeurtenis(
                           Beschrijving: "Werkingsgebieden werden niet bepaald.",
                           nameof(WerkingsgebiedenWerdenNietBepaald),
                           fixture.Scenario.WerkingsgebiedenWerdenNietBepaald,
                           fixture.MetadataInitiator,
                           fixture.MetadataTijdstip)
                   );
}
