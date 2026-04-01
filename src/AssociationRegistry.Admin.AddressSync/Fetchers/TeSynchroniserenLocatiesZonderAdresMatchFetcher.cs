namespace AssociationRegistry.Admin.AddressSync.Fetchers;

using AssociationRegistry.Admin.Schema.Locaties;
using Marten;
using Microsoft.Extensions.Logging;

public class TeSynchroniserenLocatiesZonderAdresMatchFetcher(
    ILogger<TeSynchroniserenLocatiesZonderAdresMatchFetcher> logger)
    : ITeSynchroniserenLocatiesZonderAdresMatchFetcher
{
    public async Task<LocatieZonderAdresMatchDocument[]> GetTeSynchroniserenLocatiesZonderAdresMatch(
        IDocumentSession session,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetcher (zonder adresmatch) started.");

        var zonderMatchDocs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync(cancellationToken);

        if (zonderMatchDocs.Count == 0)
        {
            logger.LogInformation("Fetcher (zonder adresmatch) found no documents.");

            return [];
        }

        return zonderMatchDocs.ToArray();
    }
}
