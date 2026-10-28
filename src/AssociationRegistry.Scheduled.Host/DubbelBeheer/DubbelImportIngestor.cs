namespace AssociationRegistry.Scheduled.Host.DubbelBeheer;

using Amazon.S3;
using Amazon.S3.Model;
using Marten;
using Microsoft.Extensions.Logging;
using Parsing;
using Schema;

/// <summary>
///     Leest het CSV-bestand uit de source bucket in, bewaart elke rij als <see cref="DubbelImportDocument" />
///     en verwijdert daarna het bestand.
///     Er wordt exact één CSV-bestand in de bucket verwacht.
///     Werd het bestand al bewaard maar niet verwijderd, dan wordt het opslaan overgeslagen en enkel het bestand verwijderd.
/// </summary>
public class DubbelImportIngestor(
    IAmazonS3 s3,
    DubbelImportOptions options,
    IDocumentStore store,
    ILogger<DubbelImportIngestor> logger
)
{
    public async Task Ingest(CancellationToken cancellationToken)
    {
        var key = await GetCsvKey(cancellationToken);

        await using var session = store.LightweightSession();

        if (await IngestAlreadyRun(cancellationToken, session, key))
        {
            logger.LogWarning("Bestand {SourceKey} werd al ingelezen, opslaan wordt overgeslagen", key);
        }
        else
        {
            using var response = await s3.GetObjectAsync(options.SourceBucket, key, cancellationToken);
            var records = DubbelCsvParser.Parse(response.ResponseStream);

            session.Store(DubbelImportDocument.Nieuw(key, records, DateTimeOffset.UtcNow).ToArray());
            await session.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Bestand {SourceKey} ingelezen met {Aantal} rijen", key, records.Count);
        }

        await s3.DeleteObjectAsync(options.SourceBucket, key, cancellationToken);
    }

    private static async Task<bool> IngestAlreadyRun(
        CancellationToken cancellationToken,
        IDocumentSession session,
        string key
    ) => await session.Query<DubbelImportDocument>().AnyAsync(x => x.SourceKey == key, cancellationToken);

    private async Task<string> GetCsvKey(CancellationToken cancellationToken)
    {
        var response = await s3.ListObjectsV2Async(
            new ListObjectsV2Request { BucketName = options.SourceBucket },
            cancellationToken
        );

        return (response.S3Objects ?? []).Single(x => x.Key.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)).Key;
    }
}
