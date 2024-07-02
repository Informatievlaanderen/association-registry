namespace AssociationRegistry.Grar.AddressSync;

using Models;

public record TeSynchroniserenLocatieAdresMessage(string VCode, List<LocatieWithAdres> LocatiesWithAdres, string IdempotenceKey);
