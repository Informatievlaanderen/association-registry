namespace AssociationRegistry.Acm.Api.Caches;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.BlobStore;
using Newtonsoft.Json;
using S3;

public class VerenigingenRepository : IVerenigingenRepository
{
    private const string BlobName = WellknownBuckets.Verenigingen.Blobs.Data;

    private readonly VerenigingenBlobClient _blobClient;

    private VerenigingenRepository(VerenigingenBlobClient blobClient,
        VerenigingenPerRijksregisternummer verenigingen)
    {
        _blobClient = blobClient;
        Verenigingen = verenigingen;
    }

    public VerenigingenPerRijksregisternummer Verenigingen { get; set; }

    public static async Task<VerenigingenRepository> Load(VerenigingenBlobClient verenigingenBlobClient) =>
        new(verenigingenBlobClient, await GetVerenigingen(verenigingenBlobClient));

    public async Task UpdateVerenigingen(VerenigingenAsDictionary jsonDictionary,
        Stream requestBodyStream, CancellationToken cancellationToken)
    {
        var verenigingen = Parse(jsonDictionary);

        if (await _blobClient.BlobExistsAsync(BlobName, cancellationToken))
            await _blobClient.DeleteBlobAsync(BlobName, cancellationToken);

        await _blobClient.CreateBlobAsync(BlobName, Metadata.None, ContentType.Parse("application/json"),
            requestBodyStream, cancellationToken);

        Verenigingen = verenigingen;
    }

    private static async Task<VerenigingenPerRijksregisternummer> GetVerenigingen(
        VerenigingenBlobClient verenigingenBlobClient)
    {

        if (!await verenigingenBlobClient.BlobExistsAsync(BlobName))
            return VerenigingenPerRijksregisternummer.Empty();

        var blobObject = await verenigingenBlobClient.GetBlobAsync(BlobName);
        var blobStream = await blobObject.OpenAsync();
        var json = await new StreamReader(blobStream).ReadToEndAsync();
        var jsonDictionary = JsonConvert.DeserializeObject<VerenigingenAsDictionary>(json);

        return Parse(jsonDictionary!);
    }

    private static VerenigingenPerRijksregisternummer Parse(VerenigingenAsDictionary dictionary)
    {
        try
        {
            return VerenigingenPerRijksregisternummer.FromVerenigingenAsDictionary(dictionary);
        }
        catch
        {
            throw new ApplicationException("Er is een fout gebeurd bij het inlezen van de verenigingenbestand.");
        }
    }
}
