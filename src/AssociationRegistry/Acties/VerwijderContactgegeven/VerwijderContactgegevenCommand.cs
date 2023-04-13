namespace AssociationRegistry.Acties.VerwijderContactgegeven;

using Vereniging;

public record VerwijderContactgegevenCommand(VCode VCode, int ContactgegevenId);
