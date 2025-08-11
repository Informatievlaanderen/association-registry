namespace AssociationRegistry.CommandHandling.Grar.NightlyAdresSync.SyncAdresLocaties;

using AssociationRegistry.Grar.Models;

public record SyncAdresLocatiesCommand(string VCode, List<LocatieWithAdres> LocatiesWithAdres, string IdempotenceKey);
