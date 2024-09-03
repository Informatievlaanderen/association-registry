namespace AssociationRegistry.PowerBi.ExportHost.HostedServices;

using Amazon.S3;
using Amazon.S3.Model;
using Admin.Schema.PowerBiExport;
using Configuration;
using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class PowerBiExportService : BackgroundService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IDocumentStore _store;
    private readonly ILogger<PowerBiExportService> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly string _bucketName;

    public PowerBiExportService(
        IAmazonS3 s3Client,
        IDocumentStore store,
        PowerBiExportOptionsSection optionsSection,
        ILogger<PowerBiExportService> logger,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _s3Client = s3Client;
        _store = store;
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
        _bucketName = optionsSection.BucketName;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var session = _store.LightweightSession();
            var docs = await session.Query<PowerBiExportDocument>().ToListAsync(cancellationToken);
            _logger.LogInformation($"Amount of docs found: {docs.Count}");
            await UploadListAsCsvToS3(docs);
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

    public async Task UploadListAsCsvToS3(IEnumerable<PowerBiExportDocument> documents)
    {
        _logger.LogInformation("UploadListAsCsvToS3 started");

        var exporter = new PowerBiDocumentExporter();

        var streamsToWrite = new List<(string, MemoryStream)>
        {
            (WellKnownFileNames.Hoofdactiviteiten, await exporter.ExportHoofdactiviteiten(documents)),
            (WellKnownFileNames.Basisgegevens, await exporter.ExportBasisgegevens(documents)),
            (WellKnownFileNames.Locaties, await exporter.ExportLocaties(documents)),
            (WellKnownFileNames.Contactgegevens, await exporter.ExportContactgegevens(documents)),
        };

        foreach (var (fileName, stream) in streamsToWrite)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = stream,
                ContentType = "text/csv",
            };

            _logger.LogInformation($"Send file {fileName} to s3.");
            await _s3Client.PutObjectAsync(putRequest);
        }
    }
}