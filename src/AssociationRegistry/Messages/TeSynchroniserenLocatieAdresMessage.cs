namespace AssociationRegistry.Messages;

using Acties.SyncAdresLocaties;
using Grar.Models;

public record TeSynchroniserenLocatieAdresMessage(string VCode, List<LocatieWithAdres> LocatiesWithAdres, string IdempotenceKey)
{
    public SyncAdresLocatiesCommand ToCommand()
        => new(VCode, LocatiesWithAdres, IdempotenceKey);
}
