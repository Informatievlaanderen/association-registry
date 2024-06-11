namespace AssociationRegistry.Acties.HeradresseerLocaties;

public record TeHeradresserenLocatiesMessage(string VCode, List<(int, string)> LocatiesMetAdres, string idempotencyKey);
