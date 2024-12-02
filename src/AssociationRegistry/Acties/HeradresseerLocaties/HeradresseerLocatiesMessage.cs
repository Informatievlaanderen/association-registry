namespace AssociationRegistry.Acties.HeradresseerLocaties;

using AssociationRegistry.Grar.Models;
using Grar.GrarUpdates.Hernummering;

public record HeradresseerLocatiesMessage(string VCode, List<TeHeradresserenLocatie> TeHeradresserenLocaties, string idempotencyKey);
