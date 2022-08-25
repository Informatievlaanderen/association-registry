using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Be.Vlaanderen.Basisregisters.BlobStore;
using Be.Vlaanderen.Basisregisters.BlobStore.Aws;

namespace AssociationRegistry.Acm.Api.S3;

public class VerenigingenBlobClient
{
    private readonly S3BlobClientOptions.Bucket _bucket;
    private readonly IBlobClient _client;

    public VerenigingenBlobClient(S3BlobClientOptions.Bucket bucket, Func<string, IBlobClient> clientBuilder)
    {
        _bucket = bucket;
        _client = clientBuilder.Invoke(bucket.Name);
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
        CancellationToken cancellationToken = default) =>
        _client.CreateBlobAsync(GetBlobName(name), metadata, contentType, content, cancellationToken);

    public Task DeleteBlobAsync(string name, CancellationToken cancellationToken = default) =>
        _client.DeleteBlobAsync(GetBlobName(name), cancellationToken);

    private BlobName GetBlobName(string name) => new(_bucket.Blobs[name]);
}
