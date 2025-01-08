namespace AssociationRegistry.DecentraalBeheer.Contactgegevens.VerwijderContactgegeven;

using AssociationRegistry.Vereniging;

public record VerwijderContactgegevenCommand(VCode VCode, int ContactgegevenId);
