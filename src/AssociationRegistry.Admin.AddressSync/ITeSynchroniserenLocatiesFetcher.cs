namespace AssociationRegistry.Admin.AddressSync;

using Marten;
using Messages;

public interface ITeSynchroniserenLocatiesFetcher
{
    Task<IEnumerable<TeSynchroniserenLocatieAdresMessage>> GetTeSynchroniserenLocaties(IDocumentSession session, CancellationToken stoppingToken);
}
