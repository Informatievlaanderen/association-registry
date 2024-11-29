namespace AssociationRegistry.Grar.GrarConsumer.TeHeradresserenLocaties;

using AssociationRegistry.Grar.Models;

public record TeHeradresserenLocatiesMessage(string VCode, List<TeHeradresserenLocatie> TeHeradresserenLocaties, string idempotencyKey);
