namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using KellermanSoftware.CompareNetObjects;
using System.Linq;
using System.Threading.Tasks;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenGewijzigd(PowerBiScenarioFixture<WerkingsgebiedenWerdenGewijzigdScenario> fixture)
    : PowerBiScenarioClassFixture<WerkingsgebiedenWerdenGewijzigdScenario>
{
    [Fact]
    public async Task ARecordIsStored_With_Hoofdactiviteiten()
    {
       var expectedHoofdactiviteiten =
            fixture.Scenario
               .WerkingsgebiedenWerdenGewijzigd
               .Werkingsgebieden
               .Select(x => new Werkingsgebied
                {
                    Naam = x.Naam,
                    Code = x.Code,
                })
               .ToArray();

        fixture.Result.Werkingsgebieden.ShouldCompare(expectedHoofdactiviteiten);
    }

    [Fact]
    public async Task ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingWerdGeregistreerd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == "WerkingsgebiedenWerdenGewijzigd");
    }
}
