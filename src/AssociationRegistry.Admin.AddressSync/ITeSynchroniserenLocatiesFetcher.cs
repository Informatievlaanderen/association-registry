namespace AssociationRegistry.Admin.AddressSync;

using Grar.AddressSync;
using Marten;

public interface ITeSynchroniserenLocatiesFetcher
{
    Task<IEnumerable<TeSynchroniserenLocatieAdresMessage>> GetTeSynchroniserenLocaties(IDocumentSession session, CancellationToken stoppingToken);
}
