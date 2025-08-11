namespace AssociationRegistry.Grar.Integration.Messages;

using AssociationRegistry.Grar.Models;
using AssociationRegistry.CommandHandling.Grar.NightlyAdresSync.SyncAdresLocaties;

public record TeSynchroniserenLocatieAdresMessage(string VCode, List<LocatieWithAdres> LocatiesWithAdres, string IdempotenceKey)
{
    public SyncAdresLocatiesCommand ToCommand()
        => new(VCode, LocatiesWithAdres, IdempotenceKey);
}
