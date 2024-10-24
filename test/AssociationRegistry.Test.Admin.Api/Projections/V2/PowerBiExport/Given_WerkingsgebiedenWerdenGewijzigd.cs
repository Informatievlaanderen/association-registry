namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Projections.PowerBiExport;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(PowerBiExportContext))]
public class Given_WerkingsgebiedenWerdenGewijzigd : IClassFixture<WerkingsgebiedenWerdenGewijzigdScenario>
{
    private readonly PowerBiExportContext _context;
    private readonly WerkingsgebiedenWerdenGewijzigdScenario _scenario;

    public Given_WerkingsgebiedenWerdenGewijzigd(
        PowerBiExportContext context,
        WerkingsgebiedenWerdenGewijzigdScenario scenario)
    {
        _context = context;
        _scenario = scenario;
    }

    [Fact]
    public async Task ARecordIsStored_With_Hoofdactiviteiten()
    {
        await using var documentSession = _context
           .Session;

        var powerBiExportDocument =
            await documentSession
                 .Query<PowerBiExportDocument>()
                 .Where(w => w.VCode == _scenario.VerenigingWerdGeregistreerd.VCode)
                 .SingleAsync();

        var expectedHoofdactiviteiten =
            _scenario
               .WerkingsgebiedenWerdenGewijzigd
               .Werkingsgebieden
               .Select(x => new Werkingsgebied()
                {
                    Naam = x.Naam,
                    Code = x.Code,
                })
               .ToArray();

        powerBiExportDocument.Werkingsgebieden.ShouldCompare(expectedHoofdactiviteiten);
    }

    [Fact]
    public async Task ARecordIsStored_With_Historiek()
    {
        await using var documentSession = _context
           .Session;

        var powerBiExportDocument =
            await documentSession
                 .Query<PowerBiExportDocument>()
                 .SingleAsync(w => w.VCode == _scenario.VerenigingWerdGeregistreerd.VCode);

        powerBiExportDocument.VCode.Should().Be(_scenario.VerenigingWerdGeregistreerd.VCode);
        powerBiExportDocument.Historiek.Should().NotBeEmpty();
        powerBiExportDocument.Historiek.Should()
                             .ContainSingle(x => x.EventType == "WerkingsgebiedenWerdenGewijzigd");
    }
}
