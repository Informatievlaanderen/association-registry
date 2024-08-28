namespace AssociationRegistry.PowerBi.ExportHost.HostedServices;

using Amazon.S3;
using Amazon.S3.Model;
using Admin.Schema.PowerBiExport;
using Configuration;
using Marten;
using Microsoft.Extensions.Hosting;

public class PowerBiExportService : BackgroundService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IDocumentStore _store;
    private readonly bool _demo;
    private readonly string _bucketName;

    public PowerBiExportService(IAmazonS3 s3Client, IDocumentStore store, PowerBiExportOptionsSection optionsSection)
    {
        _s3Client = s3Client;
        _store = store;
        _demo = optionsSection.Demo;
        _bucketName = optionsSection.BucketName;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (_demo)
        {
            var docs = new List<PowerBiExportDocument>
            {
                new()
                {
                    VCode = "V0001001",
                    HoofdactiviteitenVerenigingsloket = [
                        new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd", Code = "activiteit"},
                    ]
                },
                new()
                {
                    VCode = "V0001002",
                    HoofdactiviteitenVerenigingsloket = [
                        new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd1", Code = "activiteit1"},
                        new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd2", Code = "activiteit2"},
                        new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd3", Code = "activiteit3"},
                    ]
                }
            };
            await UploadListAsCsvToS3(docs);
        }
        else
        {
            await using var session = _store.LightweightSession();
            var docs = await session.Query<PowerBiExportDocument>().ToListAsync(cancellationToken);
            await UploadListAsCsvToS3(docs);
        }
    }

    public async Task UploadListAsCsvToS3(IEnumerable<PowerBiExportDocument> documents)
    {
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

        await _s3Client.PutObjectAsync(putRequest);
    }
}
