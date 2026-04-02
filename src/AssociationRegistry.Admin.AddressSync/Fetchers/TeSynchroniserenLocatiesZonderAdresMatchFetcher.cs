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

        return zonderMatchDocs.ToArray();
    }
}
