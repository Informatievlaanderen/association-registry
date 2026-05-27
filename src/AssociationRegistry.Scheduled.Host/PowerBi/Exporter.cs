namespace AssociationRegistry.Scheduled.Host.PowerBi;

using System.Collections.ObjectModel;
using System.Globalization;
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
        : base(exporters)
    {
    }
}

public class PowerBiDubbelDetectieExporters : ReadOnlyCollection<Exporter<PowerBiExportDubbelDetectieDocument>>
{
    public PowerBiDubbelDetectieExporters(IList<Exporter<PowerBiExportDubbelDetectieDocument>> exporters)
        : base(exporters)
    {
    }
}

public class Exporter<TSource>
{
    private readonly string _key;
    private readonly string _bucketName;
    private readonly IRecordWriter<TSource> _writer;
    private readonly IAmazonS3 _s3Client;
    private readonly PowerBIS3Logger _logger;

    public Exporter(
        string key,
        string bucketName,
        IRecordWriter<TSource> writer,
        IAmazonS3 s3Client,
        ILogger<Exporter<TSource>> logger)
    {
        _key = key;
        _bucketName = bucketName;
        _writer = writer;
        _s3Client = s3Client;
        _logger = new PowerBIS3Logger(logger, bucketName, key);
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

        _logger.LogS3UploadInformation(stream);

        try
        {
            var responseSendingToS3 = await _s3Client.PutObjectAsync(putRequest);

            _logger.LogS3UploadResponse(responseSendingToS3);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogS3Exception(ex);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
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
