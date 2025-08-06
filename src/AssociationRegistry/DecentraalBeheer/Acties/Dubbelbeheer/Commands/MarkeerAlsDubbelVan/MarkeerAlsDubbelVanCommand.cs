namespace AssociationRegistry.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;

using Vereniging;

public record MarkeerAlsDubbelVanCommand(VCode VCode, VCode VCodeAuthentiekeVereniging);
