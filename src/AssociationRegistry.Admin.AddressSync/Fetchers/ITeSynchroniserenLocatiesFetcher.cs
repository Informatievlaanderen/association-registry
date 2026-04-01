namespace AssociationRegistry.Admin.AddressSync.Fetchers;

using AssociationRegistry.Integrations.Grar.Integration.Messages;
using Marten;

public interface ITeSynchroniserenLocatiesFetcher
{
    Task<IEnumerable<TeSynchroniserenLocatieAdresMessage>> GetTeSynchroniserenLocaties(IDocumentSession session, CancellationToken stoppingToken);
}
