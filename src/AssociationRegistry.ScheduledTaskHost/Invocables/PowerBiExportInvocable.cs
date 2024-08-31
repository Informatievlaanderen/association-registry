namespace AssociationRegistry.ScheduledTaskHost.Invocables;

using Admin.Schema.PowerBiExport;
using Amazon.S3;
using Amazon.S3.Model;
using Configuration;
using Coravel.Invocable;
using Helpers;
using Marten;

public class PowerBiExportInvocable : IInvocable, ICancellableInvocable
{
    private readonly IAmazonS3 _s3Client;
    private readonly IDocumentStore _store;
    private readonly ILogger<PowerBiExportInvocable> _logger;
    private readonly bool _demo;
    private readonly string _bucketName;

    public PowerBiExportInvocable(
        IAmazonS3 s3Client,
        IDocumentStore store,
        PowerBiExportOptions options,
        ILogger<PowerBiExportInvocable> logger)
    {
        _s3Client = s3Client;
        _store = store;
        _logger = logger;
        _demo = options.Demo;
        _bucketName = options.BucketName;
    }

    public async Task Invoke()
    {
        if (_demo)
        {
            _logger.LogInformation("This is demo data.");

            var docs = new List<PowerBiExportDocument>
            {
                new()
                {
                    VCode = "V0001001",
                    HoofdactiviteitenVerenigingsloket =
                    [
                        new HoofdactiviteitVerenigingsloket() { Naam = "hoofd", Code = "activiteit" },
                    ]
                },
                new()
                {
                    VCode = "V0001002",
                    HoofdactiviteitenVerenigingsloket =
                    [
                        new HoofdactiviteitVerenigingsloket() { Naam = "hoofd1", Code = "activiteit1" },
                        new HoofdactiviteitVerenigingsloket() { Naam = "hoofd2", Code = "activiteit2" },
                        new HoofdactiviteitVerenigingsloket() { Naam = "hoofd3", Code = "activiteit3" },
                    ]
                }
            };

            await UploadListAsCsvToS3(docs);
        }
        else
        {
            await using var session = _store.LightweightSession();
            var docs = await session.Query<PowerBiExportDocument>().ToListAsync(CancellationToken);
            _logger.LogInformation($"Amount of docs found: {docs.Count}");
            await UploadListAsCsvToS3(docs);
        }
    }

    private async Task UploadListAsCsvToS3(IEnumerable<PowerBiExportDocument> documents)
    {
        _logger.LogInformation("UploadListAsCsvToS3 started");
        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        var exporter = new PowerBiDocumentExporter();

        await using var exportStream = await exporter.ExportAsync(documents);

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = WellKnownFileNames.HoofdActiviteiten,
            InputStream = exportStream,
            ContentType = "text/csv",
        };

        _logger.LogInformation("Send to s3");
        await _s3Client.PutObjectAsync(putRequest, CancellationToken);
    }

    public CancellationToken CancellationToken { get; set; }
}

public static class WellKnownFileNames
{
    public const string HoofdActiviteiten = "Hoofdactiviteiten.csv";
}
