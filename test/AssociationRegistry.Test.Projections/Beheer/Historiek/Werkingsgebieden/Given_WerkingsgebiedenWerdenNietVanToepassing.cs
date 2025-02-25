namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Werkingsgebieden;

using Admin.Schema.Historiek;
using Events;
using Scenario.Werkingsgebieden;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing(
    BeheerHistoriekScenarioFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario>
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
                           Beschrijving: "Werkingsgebieden werden niet van toepassing.",
                           nameof(WerkingsgebiedenWerdenNietVanToepassing),
                           fixture.Scenario.WerkingsgebiedenWerdenNietVanToepassing,
                           fixture.MetadataInitiator,
                           fixture.MetadataTijdstip)
                   );
}
