namespace AssociationRegistry.Admin.AddressSync.Fetchers;

using AssociationRegistry.Admin.Schema.Locaties;
using Marten;

public interface ITeSynchroniserenLocatiesZonderAdresMatchFetcher
{
    Task<LocatieZonderAdresMatchDocument[]> GetTeSynchroniserenLocatiesZonderAdresMatch(
        IDocumentSession session,
        CancellationToken cancellationToken);
}
