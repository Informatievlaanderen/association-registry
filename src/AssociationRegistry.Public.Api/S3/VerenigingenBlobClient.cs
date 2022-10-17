namespace AssociationRegistry.Public.Api.S3;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.BlobStore;
using Microsoft.Extensions.Logging;

public class VerenigingenBlobClient
{
    private readonly S3BlobClientOptions.Bucket _bucket;
    private readonly ILogger<VerenigingenBlobClient> _logger;
    private readonly IBlobClient _client;

    public VerenigingenBlobClient(S3BlobClientOptions.Bucket bucket, Func<string, IBlobClient> clientBuilder, ILogger<VerenigingenBlobClient> logger)
    {
        _bucket = bucket;
        _client = clientBuilder.Invoke(bucket.Name);
        _logger = logger;
    }

    /*public VerenigingenBlobClient(IBlobClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }*/

    public Task<BlobObject> GetBlobAsync(string name, CancellationToken cancellationToken = default) =>
        _client.GetBlobAsync(GetBlobName(name), cancellationToken);

    public Task<bool> BlobExistsAsync(string name, CancellationToken cancellationToken = default) =>
        _client.BlobExistsAsync(GetBlobName(name), cancellationToken);

    public Task CreateBlobAsync(string name, Metadata metadata, ContentType contentType, Stream content,
        CancellationToken cancellationToken = default)
    {
        var blobName = GetBlobName(name);
        _logger.LogInformation("Creating blob named `{BlobName}` in bucket `{BucketName}`",blobName, _bucket.Name);
        return _client.CreateBlobAsync(blobName, metadata, contentType, content, cancellationToken);
    }

    public Task DeleteBlobAsync(string name, CancellationToken cancellationToken = default)
    {
        var blobName = GetBlobName(name);
        _logger.LogInformation("Deleting blob named `{BlobName}` in bucket `{BucketName}`",blobName, _bucket.Name);
        return _client.DeleteBlobAsync(blobName, cancellationToken);
    }

    private BlobName GetBlobName(string name) => new(_bucket.Blobs[name]);
}
