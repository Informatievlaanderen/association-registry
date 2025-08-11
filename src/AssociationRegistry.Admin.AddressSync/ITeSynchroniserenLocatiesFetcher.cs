namespace AssociationRegistry.Admin.AddressSync;

using CommandHandling.Messages;
using Marten;

public interface ITeSynchroniserenLocatiesFetcher
{
    Task<IEnumerable<TeSynchroniserenLocatieAdresMessage>> GetTeSynchroniserenLocaties(IDocumentSession session, CancellationToken stoppingToken);
}
