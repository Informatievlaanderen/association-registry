using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.Public.Api.S3;
using AssociationRegistry.Public.Api.SearchVerenigingen;
using Be.Vlaanderen.Basisregisters.BlobStore;
using Newtonsoft.Json;

namespace AssociationRegistry.Public.Api.Caches;

public interface IVerenigingenRepository
{
    ImmutableArray<Vereniging> Verenigingen { get; set; }
    Task UpdateVerenigingen(ImmutableArray<Vereniging> newVerenigingen, Stream verenigingenStream,
        CancellationToken cancellationToken);
}

public class VerenigingenRepository : IVerenigingenRepository
{
    private const string BlobName = WellknownBuckets.Verenigingen.Blobs.AllVerenigingen;

    private readonly VerenigingenBlobClient _blobClient;

    private VerenigingenRepository(VerenigingenBlobClient blobClient,
        ImmutableArray<Vereniging> verenigingen)
    {
        _blobClient = blobClient;
        Verenigingen = verenigingen;
    }

    public ImmutableArray<Vereniging> Verenigingen { get; set; }

    public static async Task<VerenigingenRepository> Load(VerenigingenBlobClient verenigingenBlobClient) =>
        new(verenigingenBlobClient, await GetVerenigingen(verenigingenBlobClient));

    public async Task UpdateVerenigingen(ImmutableArray<Vereniging> newVerenigingen,
        Stream requestBodyStream, CancellationToken cancellationToken)
    {
        if (await _blobClient.BlobExistsAsync(BlobName, cancellationToken))
            await _blobClient.DeleteBlobAsync(BlobName, cancellationToken);

        await _blobClient.CreateBlobAsync(BlobName, Metadata.None, ContentType.Parse("application/json"),
            requestBodyStream, cancellationToken);

        Verenigingen = newVerenigingen;
    }

    private static async Task<ImmutableArray<Vereniging>> GetVerenigingen(
        VerenigingenBlobClient verenigingenBlobClient)
    {

        if (!await verenigingenBlobClient.BlobExistsAsync(BlobName))
            return ImmutableArray<Vereniging>.Empty;

        var blobObject = await verenigingenBlobClient.GetBlobAsync(BlobName);
        var blobStream = await blobObject.OpenAsync();
        var json = await new StreamReader(blobStream).ReadToEndAsync();
        return JsonConvert.DeserializeObject<ImmutableArray<Vereniging>>(json);
    }
}
