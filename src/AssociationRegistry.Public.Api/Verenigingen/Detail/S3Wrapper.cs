namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Amazon.S3;
using Amazon.S3.Model;
using Infrastructure.ConfigurationBindings;
using System.Net.Mime;

public class S3Wrapper : IS3Wrapper
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _key;

    public S3Wrapper(IAmazonS3 s3Client, AppSettings appSettings)
    {
        _s3Client = s3Client;
        _bucketName = appSettings.Publiq.BucketName;
        _key = appSettings.Publiq.Key;
    }

    public async Task<string> GetPreSignedUrlAsync(CancellationToken cancellationToken)
        => await _s3Client.GetPreSignedURLAsync(new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = _key,
            Expires = DateTime.Now.AddMinutes(5),
            Verb = HttpVerb.GET,
        });

    public async Task PutAsync(Stream stream, CancellationToken cancellationToken)
    {
        stream.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = _key,
            InputStream = stream,
            ContentType = MediaTypeNames.Text.Plain,
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);
    }
}
