namespace AssociationRegistry.Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;

using Models;

public record TeHeradresserenLocatiesMessage(string VCode, List<TeHeradresserenLocatie> TeHeradresserenLocaties, string idempotencyKey);
