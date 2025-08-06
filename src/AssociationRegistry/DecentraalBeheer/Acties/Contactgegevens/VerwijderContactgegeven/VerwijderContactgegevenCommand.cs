namespace AssociationRegistry.DecentraalBeheer.Acties.Contactgegevens.VerwijderContactgegeven;

using Vereniging;

public record VerwijderContactgegevenCommand(VCode VCode, int ContactgegevenId);
