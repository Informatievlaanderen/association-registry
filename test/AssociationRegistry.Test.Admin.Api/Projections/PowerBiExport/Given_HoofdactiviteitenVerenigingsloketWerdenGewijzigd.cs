namespace AssociationRegistry.Test.Admin.Api.Projections.PowerBiExport;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using Events;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_HoofdactiviteitenVerenigingsloketWerdenGewijzigd : IClassFixture<HoofdactiviteitenWerdenGewijzigdScenario>
{
    private readonly ProjectionContext _context;
    private readonly HoofdactiviteitenWerdenGewijzigdScenario _scenario;

    public Given_HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
        ProjectionContext context,
        HoofdactiviteitenWerdenGewijzigdScenario scenario)
    {
        _context = context;
        _scenario = scenario;
    }

    [Fact]
    public async Task ARecordIsStored_With_Hoofdactiviteiten()
    {
        var powerBiExportDocument =
            await _context
                 .Session
                 .Query<PowerBiExportDocument>()
                 .Where(w => w.VCode == _scenario.VerenigingWerdGeregistreerd.VCode)
                 .SingleAsync();

        var expectedHoofdactiviteiten =
            _scenario
               .HoofdactiviteitenVerenigingsloketWerdenGewijzigd
               .HoofdactiviteitenVerenigingsloket
               .Select(x => new HoofdactiviteitVerenigingsloket()
                {
                    Naam = x.Naam,
                    Code = x.Code,
                })
               .ToArray();

        powerBiExportDocument.HoofdactiviteitenVerenigingsloket.ShouldCompare(expectedHoofdactiviteiten);
    }
}
