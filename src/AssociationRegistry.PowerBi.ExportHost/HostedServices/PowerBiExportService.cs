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
    private readonly IEnumerable<Exporter<PowerBiExportDocument>> _exporters;
    private readonly IEnumerable<Exporter<PowerBiExportDubbelDetectieDocument>> _ddExporters;
    public PowerBiExportService(
        IDocumentStore store,
        ILogger<PowerBiExportService> logger,
        IHostApplicationLifetime hostApplicationLifetime,
        IEnumerable<Exporter<PowerBiExportDocument>> exporters,
        IEnumerable<Exporter<PowerBiExportDubbelDetectieDocument>> ddExporters)
    {
        _store = store;
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
        _exporters = exporters;
        _ddExporters = ddExporters;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("PowerBi export started");

        try
        {
            var ddDocs = await GetDocuments<PowerBiExportDubbelDetectieDocument>(cancellationToken);
            _logger.LogInformation("Found {Count} PowerBiExportDocumentDubbelDetectie items.", ddDocs.Count);
            foreach (var exporter in _ddExporters)
            {
                await exporter.Export(ddDocs);
            }

            var docs = await GetDocuments<PowerBiExportDocument>(cancellationToken);
            _logger.LogInformation("Found {Count} PowerBiExportDocument items.", docs.Count);
            foreach (var exporter in _exporters)
            {
                await exporter.Export(docs);
            }

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

    private async Task<IReadOnlyList<TDoc>> GetDocuments<TDoc>(CancellationToken ct)
    {
        await using var session = _store.LightweightSession();
        return await session.Query<TDoc>().ToListAsync(ct);
    }
}
