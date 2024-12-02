namespace AssociationRegistry.Acties.HeradresseerLocaties;

using AssociationRegistry.Grar.Models;

public record HeradresseerLocatiesMessage(string VCode, List<TeHeradresserenLocatie> TeHeradresserenLocaties, string idempotencyKey);
