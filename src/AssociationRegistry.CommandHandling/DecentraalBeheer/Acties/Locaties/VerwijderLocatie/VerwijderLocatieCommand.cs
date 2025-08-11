namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.VerwijderLocatie;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record VerwijderLocatieCommand(VCode VCode, int LocatieId);
