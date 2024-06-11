namespace AssociationRegistry.Grar.HeradresseerLocaties;

using Models;

public record TeHeradresserenLocatiesMessage(string VCode, List<LocatieIdWithAdresId> LocatiesMetAdres, string idempotencyKey);
