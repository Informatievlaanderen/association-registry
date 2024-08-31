namespace AssociationRegistry.ScheduledTaskHost.Helpers;

using AssociationRegistry.Grar.AddressSync;
using Marten;

public interface ITeSynchroniserenLocatiesFetcher
{
    Task<IEnumerable<TeSynchroniserenLocatieAdresMessage>> GetTeSynchroniserenLocaties(IDocumentSession session, CancellationToken stoppingToken);
}
