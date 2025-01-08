namespace AssociationRegistry.Grar.GrarConsumer.Messaging.HeradresseerLocaties;

using AssociationRegistry.Grar.GrarUpdates.Hernummering;

public record HeradresseerLocatiesMessage(string VCode, List<TeHeradresserenLocatie> TeHeradresserenLocaties, string idempotencyKey);
