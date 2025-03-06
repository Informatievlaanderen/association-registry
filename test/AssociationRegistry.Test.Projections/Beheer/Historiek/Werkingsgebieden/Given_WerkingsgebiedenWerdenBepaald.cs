namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Werkingsgebieden;

using Admin.Schema.Historiek;
using Events;
using Scenario.Werkingsgebieden;
using System.Linq;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenBepaald(BeheerHistoriekScenarioFixture<WerkingsgebiedenWerdenBepaaldScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<WerkingsgebiedenWerdenBepaaldScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: "Werkingsgebieden werden bepaald.",
                                               nameof(WerkingsgebiedenWerdenBepaald),
                                               fixture.Scenario.WerkingsgebiedenWerdenBepaald,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
