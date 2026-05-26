namespace AssociationRegistry.Scheduled.Host.PowerBi;

using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using AssociationRegistry.Admin.Schema.PowerBiExport;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Writers;

public class PowerBiExporters : ReadOnlyCollection<Exporter<PowerBiExportDocument>>
{
    public PowerBiExporters(IList<Exporter<PowerBiExportDocument>> exporters)
        : base(exporters) { }
}

public class PowerBiDubbelDetectieExporters : ReadOnlyCollection<Exporter<PowerBiExportDubbelDetectieDocument>>
{
    public PowerBiDubbelDetectieExporters(IList<Exporter<PowerBiExportDubbelDetectieDocument>> exporters)
        : base(exporters) { }
}

public class Exporter<TSource>
{
    private readonly string _key;
    private readonly string _bucketName;
    private readonly IRecordWriter<TSource> _writer;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<Exporter<TSource>> _logger;

    public Exporter(
        string key,
        string bucketName,
        IRecordWriter<TSource> writer,
        IAmazonS3 s3Client,
        ILogger<Exporter<TSource>> logger
    )
    {
        _key = key;
        _bucketName = bucketName;
        _writer = writer;
        _s3Client = s3Client;
        _logger = logger;
    }

    public async Task Export(IEnumerable<TSource> docs)
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

        _logger.LogInformation(
            "Sending {MemoryStreamByteCount} bytes to s3 bucket {BucketName} with key {Key}.",
            stream.Length,
            _key,
            _bucketName
        );

        try
        {
            var responseSendingToS3 = await _s3Client.PutObjectAsync(putRequest);

            if (responseSendingToS3.HttpStatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation(
                    "Successfully uploaded file to S3 bucket {BucketName} with key {Key}.",
                    _bucketName,
                    _key
                );
            }
            else
            {
                _logger.LogError(
                    "Upload to S3 returned unexpected status code {StatusCode} for bucket {BucketName} and key {Key}.",
                    responseSendingToS3.HttpStatusCode,
                    _bucketName,
                    _key
                );
            }
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(
                ex,
                "AWS S3 error while uploading to bucket {BucketName} with key {Key}. StatusCode: {StatusCode}, Exception: {Exception}",
                _bucketName,
                _key,
                ex.StatusCode,
                ex.InnerException
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while uploading to bucket {BucketName} with key {Key}. Exception: {InnerException}",
                _bucketName,
                _key,
                ex.InnerException
            );
        }
    }

    private static async Task<MemoryStream> WriteToStream(
        IEnumerable<TSource> docs,
        IRecordWriter<TSource> recordWriter
    )
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
