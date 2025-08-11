namespace AssociationRegistry.CommandHandling.Grar.NightlyAdresSync.SyncAdresLocaties;

using AssociationRegistry.Grar.Models;
using System.Collections.Generic;

public record SyncAdresLocatiesCommand(string VCode, List<LocatieWithAdres> LocatiesWithAdres, string IdempotenceKey);
