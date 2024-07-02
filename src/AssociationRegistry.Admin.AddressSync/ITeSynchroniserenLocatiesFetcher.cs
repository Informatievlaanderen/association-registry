namespace AssociationRegistry.Admin.AddressSync;

using Grar.AddressSync;
using Marten;

public interface ITeSynchroniserenLocatiesFetcher
{
    Task<IEnumerable<SynchroniseerLocatieMessage>> GetTeSynchroniserenLocaties(IDocumentSession session, CancellationToken stoppingToken);
}
