namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using KellermanSoftware.CompareNetObjects;

[Collection(nameof(ProjectionContext))]
public class Given_HoofdactiviteitenVerenigingsloketWerdenGewijzigd(PowerBiScenarioFixture<HoofdactiviteitenWerdenGewijzigdScenario> fixture)
    : PowerBiScenarioClassFixture<HoofdactiviteitenWerdenGewijzigdScenario>
{
    [Fact]
    public async ValueTask ARecordIsStored_With_Hoofdactiviteiten()
    {
       var expectedHoofdactiviteiten =
            fixture.Scenario
               .HoofdactiviteitenVerenigingsloketWerdenGewijzigd
               .HoofdactiviteitenVerenigingsloket
               .Select(x => new HoofdactiviteitVerenigingsloket
                {
                    Naam = x.Naam,
                    Code = x.Code,
                })
               .ToArray();

        fixture.Result.HoofdactiviteitenVerenigingsloket.ShouldCompare(expectedHoofdactiviteiten);
    }

    [Fact]
    public async ValueTask ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingWerdGeregistreerd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek.Should()
               .ContainSingle(x => x.EventType == "HoofdactiviteitenVerenigingsloketWerdenGewijzigd");
    }
}
