namespace AssociationRegistry.Acties.MarkeerAlsDubbelVan;

using Vereniging;

public record MarkeerAlsDubbelVanCommand(VCode VCode, VCode VCodeAuthentiekeVereniging);
