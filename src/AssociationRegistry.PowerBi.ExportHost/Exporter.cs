namespace AssociationRegistry.PowerBi.ExportHost;

using Amazon.S3;
using Amazon.S3.Model;
using Admin.Schema.PowerBiExport;
using Writers;
using CsvHelper;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

public class Exporter
{
    private readonly string _key;
    private readonly string _bucketName;
    private readonly IRecordWriter _writer;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<Exporter> _logger;

    public Exporter(string key, string bucketName, IRecordWriter writer, IAmazonS3 s3Client, ILogger<Exporter> logger)
    {
        _key = key;
        _bucketName = bucketName;
        _writer = writer;
        _s3Client = s3Client;
        _logger = logger;
    }

    public async Task Export(IEnumerable<PowerBiExportDocument> docs)
    {
        var stream = await WriteToStream(docs, _writer);

        await SendStreamToS3(stream);
    }

    private async Task SendStreamToS3(MemoryStream stream)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = _key,
            InputStream = stream,
            ContentType = "text/csv",
        };

        _logger.LogInformation("Sending {MemoryStreamByteCount} bytes to s3 bucket {BucketName} with key {Key}.", stream.Length, _key, _bucketName);
        await _s3Client.PutObjectAsync(putRequest);
    }

    private async Task<MemoryStream> WriteToStream(IEnumerable<PowerBiExportDocument> docs, IRecordWriter recordWriter)
    {
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

        await recordWriter.Write(docs, csvWriter);
        await csvWriter.FlushAsync();

        return await CopyStream(memoryStream);
    }

    private static async Task<MemoryStream> CopyStream(MemoryStream memoryStream)
    {
        memoryStream.Position = 0;

        var exportStream = new MemoryStream();
        await memoryStream.CopyToAsync(exportStream);
        exportStream.Position = 0;

        return exportStream;
    }
}
