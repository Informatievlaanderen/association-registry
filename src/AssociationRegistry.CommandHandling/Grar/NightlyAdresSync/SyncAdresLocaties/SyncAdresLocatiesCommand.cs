namespace AssociationRegistry.Grar.NightlyAdresSync.SyncAdresLocaties;

using Models;

public record SyncAdresLocatiesCommand(string VCode, List<LocatieWithAdres> LocatiesWithAdres, string IdempotenceKey);
