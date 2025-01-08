namespace AssociationRegistry.Acties.AdresSync.SyncAdresLocaties;

using AssociationRegistry.Grar.Models;

public record SyncAdresLocatiesCommand(string VCode, List<LocatieWithAdres> LocatiesWithAdres, string IdempotenceKey);
