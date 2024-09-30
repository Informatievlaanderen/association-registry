namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(PowerBiExportContext))]
public class Given_MultipleFeitelijkeVerenigingenWerdenGeregistreerd : IClassFixture<MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario>
{
    private readonly MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario _setup;
    private readonly PowerBiExportContext _context;
    private ComparisonConfig _compareVCodeOnly;

    public Given_MultipleFeitelijkeVerenigingenWerdenGeregistreerd(
        PowerBiExportContext context,
        MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario setup)
    {
        _context = context;
        _setup = setup;
    }

    [Fact]
    public async Task ARecordIsStoredForEachVCode()
    {
        await using var session = _context.Session;
        foreach (var feitelijkeVerenigingWerdGeregistreerd in _setup.VerenigingenwerdenGeregistreerd)
        {
            var powerBiExportDocument =
                await session.Query<PowerBiExportDocument>()
                             .SingleAsync(x => x.VCode == feitelijkeVerenigingWerdGeregistreerd.VCode);

            powerBiExportDocument.Should().NotBeNull();
        }
    }
}
