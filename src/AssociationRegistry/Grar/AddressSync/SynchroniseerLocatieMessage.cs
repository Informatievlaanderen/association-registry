namespace AssociationRegistry.Grar.AddressSync;

using Models;

public record SynchroniseerLocatieMessage(string VCode, List<LocatieWithAdres> LocatieWithAdres, string IdempotenceKey);
