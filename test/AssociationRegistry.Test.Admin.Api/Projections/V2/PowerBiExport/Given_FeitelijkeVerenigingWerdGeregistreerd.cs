namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Projections.PowerBiExport;
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
        await using var documentSession = _context
           .Session;

        var powerBiExportDocument =
            await documentSession
                 .Query<PowerBiExportDocument>()
                 .SingleAsync(x => x.VCode == _scenario.VerenigingWerdGeregistreerd.VCode);

        powerBiExportDocument.VCode.Should().Be(_scenario.VerenigingWerdGeregistreerd.VCode);
        powerBiExportDocument.Naam.Should().Be(_scenario.VerenigingWerdGeregistreerd.Naam);
    }

    [Fact]
    public async Task ARecordIsStored_With_Hoofdactiviteiten()
    {
        await using var documentSession = _context
           .Session;

        var powerBiExportDocument =
            await documentSession
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
    public async Task ARecordIsStored_With_Werkingsgebieden()
    {
        await using var documentSession = _context
           .Session;

        var powerBiExportDocument =
            await documentSession
                 .Query<PowerBiExportDocument>()
                 .SingleAsync(x => x.VCode == _scenario.VerenigingWerdGeregistreerd.VCode);

        var expectedHoofdactiviteiten =
            _scenario
               .VerenigingWerdGeregistreerd
               .Werkingsgebieden!
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
    }

}
