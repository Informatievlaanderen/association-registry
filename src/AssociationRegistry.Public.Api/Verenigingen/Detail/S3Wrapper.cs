namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Net.Mime;

public class S3Wrapper : IS3Wrapper
{
    private readonly IAmazonS3 _s3Client;

    public S3Wrapper(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task GetAsync(string bucketName, string key, Stream stream, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public async Task<string> GetPreSignedUrlAsync(string bucketName, string key, CancellationToken cancellationToken)
        => await _s3Client.GetPreSignedURLAsync(new()
        {
            BucketName = bucketName,
            Key = key,
            ContentType = MediaTypeNames.Text.Plain,
            Expires = DateTime.Now.AddMinutes(5),
        });

    public async Task PutAsync(string bucketName, string key, Stream stream, CancellationToken cancellationToken)
    {
        stream.Position = 0;
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = stream,
            ContentType = MediaTypeNames.Text.Plain,
        };
        await _s3Client.PutObjectAsync(request, cancellationToken);
    }
}
