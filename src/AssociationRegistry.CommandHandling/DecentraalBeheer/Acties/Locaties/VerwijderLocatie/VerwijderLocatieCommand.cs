namespace AssociationRegistry.DecentraalBeheer.Acties.Locaties.VerwijderLocatie;

using Vereniging;

public record VerwijderLocatieCommand(VCode VCode, int LocatieId);
