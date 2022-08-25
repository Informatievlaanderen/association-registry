using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.BlobStore;

namespace AssociationRegistry.Acm.Api.S3;

public class VerenigingenBlobClient : IBlobClient
{
    private readonly IBlobClient _client;

    public VerenigingenBlobClient(IBlobClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public Task<BlobObject> GetBlobAsync(BlobName name, CancellationToken cancellationToken = default) => _client.GetBlobAsync(name, cancellationToken);

    public Task<bool> BlobExistsAsync(BlobName name, CancellationToken cancellationToken = default) => _client.BlobExistsAsync(name, cancellationToken);

    public Task CreateBlobAsync(BlobName name, Metadata metadata, ContentType contentType, Stream content,
        CancellationToken cancellationToken = default) =>
        _client.CreateBlobAsync(name, metadata, contentType, content, cancellationToken);

    public Task DeleteBlobAsync(BlobName name, CancellationToken cancellationToken = default) => _client.DeleteBlobAsync(name, cancellationToken);
}
