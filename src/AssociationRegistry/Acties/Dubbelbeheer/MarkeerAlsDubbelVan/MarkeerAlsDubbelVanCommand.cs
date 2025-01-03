namespace AssociationRegistry.Acties.Dubbelbeheer.MarkeerAlsDubbelVan;

using AssociationRegistry.Vereniging;

public record MarkeerAlsDubbelVanCommand(VCode VCode, VCode VCodeAuthentiekeVereniging);
