namespace AssociationRegistry.Scheduled.Host.PowerBi;

using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;

public class PowerBIS3Logger
{
    private readonly ILogger _logger;
    private readonly string _key;
    private readonly string _bucketName;

    public PowerBIS3Logger(ILogger logger, string bucketName, string key)
    {
        _logger = logger;
        _bucketName = bucketName;
        _key = key;
    }

    public void LogS3UploadInformation(MemoryStream stream)
    {
        _logger.LogInformation(
            "Sending {MemoryStreamByteCount} bytes to s3 bucket {BucketName} with key {Key}.",
            stream.Length,
            _bucketName,
            _key
        );
    }

    public void LogS3UploadResponse(PutObjectResponse responseSendingToS3)
    {
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

    public void LogException(Exception ex)
    {
        _logger.LogError(
            ex,
            "Unexpected error while uploading to bucket {BucketName} with key {Key}. Exception: {InnerException}",
            _bucketName,
            _key,
            ex.InnerException
        );
    }

    public void LogS3Exception(AmazonS3Exception ex)
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
}
