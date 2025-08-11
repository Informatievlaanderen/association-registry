namespace AssociationRegistry.Admin.AddressSync;

using AssociationRegistry.Grar.Integration.Messages;
using Marten;

public interface ITeSynchroniserenLocatiesFetcher
{
    Task<IEnumerable<TeSynchroniserenLocatieAdresMessage>> GetTeSynchroniserenLocaties(IDocumentSession session, CancellationToken stoppingToken);
}
