namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Werkingsgebieden;

using Admin.Schema.Historiek;
using Events;
using Scenario;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenGewijzigd(BeheerHistoriekScenarioFixture<WerkingsgebiedenWerdenGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<WerkingsgebiedenWerdenGewijzigdScenario>
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
                           Beschrijving: "Werkingsgebieden werden gewijzigd.",
                           nameof(WerkingsgebiedenWerdenGewijzigd),
                           fixture.Scenario.WerkingsgebiedenWerdenGewijzigd,
                           fixture.Context.MetadataInitiator,
                           fixture.Context.MetadataTijdstip)
                   );
}
