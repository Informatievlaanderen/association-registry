namespace AssociationRegistry.Messages;

using Grar.Models;
using Grar.NightlyAdresSync.SyncAdresLocaties;

public record TeSynchroniserenLocatieAdresMessage(string VCode, List<LocatieWithAdres> LocatiesWithAdres, string IdempotenceKey)
{
    public SyncAdresLocatiesCommand ToCommand()
        => new(VCode, LocatiesWithAdres, IdempotenceKey);
}
