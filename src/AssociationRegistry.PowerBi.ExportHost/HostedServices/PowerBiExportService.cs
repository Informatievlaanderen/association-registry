namespace AssociationRegistry.PowerBi.ExportHost.HostedServices;

using Admin.Schema.PowerBiExport;
using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class PowerBiExportService : BackgroundService
{
    private readonly IDocumentStore _store;
    private readonly ILogger<PowerBiExportService> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IEnumerable<Exporter> _exporters;

    public PowerBiExportService(
        IDocumentStore store,
        ILogger<PowerBiExportService> logger,
        IHostApplicationLifetime hostApplicationLifetime,
        IEnumerable<Exporter> exporters)
    {
        _store = store;
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
        _exporters = exporters;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("PowerBi export started");

        try
        {
            var docs = await GetPowerBiExportDocuments(cancellationToken);

            _logger.LogInformation("Amount of PowerBiExportDocuments found: {AmountOfPowerBiExportDocuments}", docs.Count);

            foreach (var exporter in _exporters)
                await exporter.Export(docs);

            _logger.LogInformation("PowerBi export succeeded");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PowerBi export failed");

            throw;
        }
        finally
        {
            _hostApplicationLifetime.StopApplication();
        }
    }

    private async Task<IReadOnlyList<PowerBiExportDocument>> GetPowerBiExportDocuments(CancellationToken cancellationToken)
    {
        await using var session = _store.LightweightSession();
        var docs = await session.Query<PowerBiExportDocument>().ToListAsync(cancellationToken);

        return docs;
    }
}
