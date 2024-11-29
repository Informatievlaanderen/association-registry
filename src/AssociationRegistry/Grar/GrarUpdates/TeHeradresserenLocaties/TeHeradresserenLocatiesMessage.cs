namespace AssociationRegistry.Grar.GrarUpdates.TeHeradresserenLocaties;

using AssociationRegistry.Grar.Models;

public record TeHeradresserenLocatiesMessage(string VCode, List<TeHeradresserenLocatie> TeHeradresserenLocaties, string idempotencyKey);
