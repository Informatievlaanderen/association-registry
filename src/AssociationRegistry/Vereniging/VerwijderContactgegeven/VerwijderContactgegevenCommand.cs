namespace AssociationRegistry.Vereniging.VerwijderContactgegeven;

using AssociationRegistry.VCodes;

public record VerwijderContactgegevenCommand(VCode VCode, int ContactgegevenId);
