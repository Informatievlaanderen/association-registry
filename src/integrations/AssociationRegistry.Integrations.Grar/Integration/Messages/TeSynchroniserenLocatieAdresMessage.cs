namespace AssociationRegistry.Integrations.Grar.Integration.Messages;

using AssociationRegistry.Grar.Models;

public record TeSynchroniserenLocatieAdresMessage(string VCode, List<LocatieWithAdres> LocatiesWithAdres, string IdempotenceKey)
{

}
