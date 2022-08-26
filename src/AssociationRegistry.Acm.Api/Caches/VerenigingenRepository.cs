namespace AssociationRegistry.Acm.Api.Caches;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Api.VerenigingenPerRijksregisternummer;
using Be.Vlaanderen.Basisregisters.BlobStore;
using Newtonsoft.Json;
using S3;

public class VerenigingenAsDictionary : Dictionary<string, Dictionary<string, string>>
{
}

public class VerenigingenPerRijksregisternummer
{
    private readonly Dictionary<string, ImmutableArray<Vereniging>> _data;

    private VerenigingenPerRijksregisternummer(Dictionary<string, ImmutableArray<Vereniging>> data)
    {
        _data = data;
    }

    public ImmutableArray<Vereniging> this[string rijksRegisterNummer] =>
        _data.ContainsKey(rijksRegisterNummer) ? _data[rijksRegisterNummer] : ImmutableArray<Vereniging>.Empty;

    public static VerenigingenPerRijksregisternummer Empty() =>
        new(new Dictionary<string, ImmutableArray<Vereniging>>());

    public static VerenigingenPerRijksregisternummer
        FromVerenigingenAsDictionary(VerenigingenAsDictionary dictionary) =>
        new(dictionary.Select(
                kv =>
                    (kv.Key, kv.Value.Select(
                            x => new Vereniging(x.Key, x.Value))
                        .ToImmutableArray()
                    ))
            .ToDictionary(x => x.Key, x => x.Item2));
}

public interface IVerenigingenRepository
{
    VerenigingenPerRijksregisternummer Verenigingen { get; set; }
    Task UpdateVerenigingen(VerenigingenAsDictionary verenigingenAsDictionary, Stream verenigingenStream, CancellationToken cancellationToken);
}

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

    private static async Task<VerenigingenPerRijksregisternummer> GetVerenigingen(
        VerenigingenBlobClient verenigingenBlobClient)
    {
        var blobName = new BlobName(WellknownBuckets.Verenigingen.Blobs.Data);

        if (!await verenigingenBlobClient.BlobExistsAsync(blobName))
            return VerenigingenPerRijksregisternummer.Empty();

        var blobObject = await verenigingenBlobClient.GetBlobAsync(blobName);
        var blobStream = await blobObject.OpenAsync();
        var json = await new StreamReader(blobStream).ReadToEndAsync();
        var jsonDictionary = JsonConvert.DeserializeObject<VerenigingenAsDictionary>(json);

        return Parse(jsonDictionary!);
    }

    public async Task UpdateVerenigingen(VerenigingenAsDictionary jsonDictionary,
        Stream requestBodyStream, CancellationToken cancellationToken)
    {
        var verenigingen = Parse(jsonDictionary);

        await _blobClient.DeleteBlobAsync(BlobName, cancellationToken);
        await _blobClient.CreateBlobAsync(BlobName, Metadata.None, ContentType.Parse("application/json"),
            requestBodyStream, cancellationToken);

        Verenigingen = verenigingen;
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
