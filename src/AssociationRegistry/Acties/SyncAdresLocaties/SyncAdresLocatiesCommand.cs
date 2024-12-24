namespace AssociationRegistry.Acties.SyncAdresLocaties;

using Grar.Models;

public record SyncAdresLocatiesCommand(string VCode, List<LocatieWithAdres> LocatiesWithAdres, string IdempotenceKey);
