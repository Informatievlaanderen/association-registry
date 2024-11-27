namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using KellermanSoftware.CompareNetObjects;
using Marten;

[Collection(nameof(ProjectionContext))]
public class Given_MultipleFeitelijkeVerenigingenWerdenGeregistreerd : IClassFixture<
    MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario>
{
    private readonly MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario _setup;
    private readonly ProjectionContext _context;
    private ComparisonConfig _compareVCodeOnly;

    public Given_MultipleFeitelijkeVerenigingenWerdenGeregistreerd(
        ProjectionContext context,
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
