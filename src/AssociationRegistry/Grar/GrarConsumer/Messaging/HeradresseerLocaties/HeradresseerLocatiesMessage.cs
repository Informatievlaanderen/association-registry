namespace AssociationRegistry.Grar.GrarConsumer.Messaging.HeradresseerLocaties;

using GrarUpdates.Hernummering;

public record HeradresseerLocatiesMessage(string VCode, List<TeHeradresserenLocatie> TeHeradresserenLocaties, string idempotencyKey);
