namespace AssociationRegistry.PowerBi.ExportHost.HostedServices;

using Amazon.S3;
using Amazon.S3.Model;
using AssociationRegistry.Admin.Schema.PowerBiExport;
using Configuration;
using Marten;
using Microsoft.Extensions.Hosting;

public class PowerBiExportService : BackgroundService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IDocumentStore _store;
    private readonly string _bucketName;

    public PowerBiExportService(IAmazonS3 s3Client, IDocumentStore store, PowerBiExportOptionsSection optionsSection)
    {
        _s3Client = s3Client;
        _store = store;
        _bucketName = optionsSection.BucketName;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var session = _store.LightweightSession();
        var docs = await session.Query<PowerBiExportDocument>().ToListAsync(cancellationToken);
        await UploadListAsCsvToS3(docs);
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
