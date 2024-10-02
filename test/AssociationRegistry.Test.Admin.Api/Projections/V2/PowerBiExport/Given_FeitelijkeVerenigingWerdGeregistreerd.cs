namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(PowerBiExportContext))]
public class Given_FeitelijkeVerenigingWerdGeregistreerd : IClassFixture<FeitelijkeVerenigingWerdGeregistreerdScenario>
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly PowerBiExportContext _context;

    public Given_FeitelijkeVerenigingWerdGeregistreerd(
        PowerBiExportContext context,
        FeitelijkeVerenigingWerdGeregistreerdScenario scenario)
    {
        _context = context;
        _scenario = scenario;
    }

    [Fact]
    public async Task ARecordIsStored_With_VCodeAndNaam()
    {
        var powerBiExportDocument =
            await _context
                 .Session
                 .Query<PowerBiExportDocument>()
                 .SingleAsync(x => x.VCode == _scenario.VerenigingWerdGeregistreerd.VCode);

        powerBiExportDocument.VCode.Should().Be(_scenario.VerenigingWerdGeregistreerd.VCode);
        powerBiExportDocument.Naam.Should().Be(_scenario.VerenigingWerdGeregistreerd.Naam);
    }

    [Fact]
    public async Task ARecordIsStored_With_Hoofdactiviteiten()
    {
        var powerBiExportDocument =
            await _context
                 .Session
                 .Query<PowerBiExportDocument>()
                 .SingleAsync(x => x.VCode == _scenario.VerenigingWerdGeregistreerd.VCode);

        var expectedHoofdactiviteiten =
            _scenario
               .VerenigingWerdGeregistreerd
               .HoofdactiviteitenVerenigingsloket
               .Select(x => new HoofdactiviteitVerenigingsloket()
                {
                    Naam = x.Naam,
                    Code = x.Code,
                })
               .ToArray();

        powerBiExportDocument.HoofdactiviteitenVerenigingsloket.ShouldCompare(expectedHoofdactiviteiten);
    }

    [Fact]
    public async Task ARecordIsStored_With_Historiek()
    {
        var powerBiExportDocument =
            await _context
                 .Session
                 .Query<PowerBiExportDocument>()
                 .SingleAsync(w => w.VCode == _scenario.VerenigingWerdGeregistreerd.VCode);

        powerBiExportDocument.VCode.Should().Be(_scenario.VerenigingWerdGeregistreerd.VCode);
        powerBiExportDocument.Historiek.Should().NotBeEmpty();
    }

}
