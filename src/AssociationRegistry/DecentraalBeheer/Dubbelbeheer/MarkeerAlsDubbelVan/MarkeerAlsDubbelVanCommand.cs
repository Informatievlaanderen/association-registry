namespace AssociationRegistry.DecentraalBeheer.Dubbelbeheer.MarkeerAlsDubbelVan;

using AssociationRegistry.Vereniging;

public record MarkeerAlsDubbelVanCommand(VCode VCode, VCode VCodeAuthentiekeVereniging);
