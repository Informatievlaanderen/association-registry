﻿namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Projections.PowerBiExport;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(PowerBiExportContext))]
public class Given_HoofdactiviteitenVerenigingsloketWerdenGewijzigd : IClassFixture<HoofdactiviteitenWerdenGewijzigdScenario>
{
    private readonly PowerBiExportContext _context;
    private readonly HoofdactiviteitenWerdenGewijzigdScenario _scenario;

    public Given_HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
        PowerBiExportContext context,
        HoofdactiviteitenWerdenGewijzigdScenario scenario)
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
                             .ContainSingle(x => x.EventType == "HoofdactiviteitenVerenigingsloketWerdenGewijzigd");
    }
}
