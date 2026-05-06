namespace AssociationRegistry.Scheduled.Host.PowerBi;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

public class PowerBiExportJob : IJob
{
    public const string JobName = "powerbi-export-runner";
    private readonly IDocumentStore _store;
    private readonly ILogger<PowerBiExportJob> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IEnumerable<Exporter<PowerBiExportDocument>> _exporters;
    private readonly IEnumerable<Exporter<PowerBiExportDubbelDetectieDocument>> _ddExporters;

    public PowerBiExportJob(
        IDocumentStore store,
        ILogger<PowerBiExportJob> logger,
        IHostApplicationLifetime hostApplicationLifetime,
        PowerBiExporters exporters,
        PowerBiDubbelDetectieExporters ddExporters)
    {
        _store = store;
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
        _exporters = exporters;
        _ddExporters = ddExporters;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("PowerBi export started");

        try
        {
            var ddDocs = await GetDocuments<PowerBiExportDubbelDetectieDocument>();
            _logger.LogInformation("Found {Count} PowerBiExportDocumentDubbelDetectie items.", ddDocs.Count);

            foreach (var exporter in _ddExporters)
            {
                await exporter.Export(ddDocs);
            }

            var docs = await GetDocuments<PowerBiExportDocument>();
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

    private async Task<IReadOnlyList<TDoc>> GetDocuments<TDoc>()
    {
        await using var session = _store.LightweightSession();

        return await session.Query<TDoc>().ToListAsync();
    }
}
