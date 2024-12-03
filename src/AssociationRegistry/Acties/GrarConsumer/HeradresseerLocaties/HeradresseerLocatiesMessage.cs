namespace AssociationRegistry.Acties.GrarConsumer.HeradresseerLocaties;

using AssociationRegistry.Grar.GrarUpdates.Hernummering;

public record HeradresseerLocatiesMessage(string VCode, List<TeHeradresserenLocatie> TeHeradresserenLocaties, string idempotencyKey);
