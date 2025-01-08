namespace AssociationRegistry.Acties.Locaties.VerwijderLocatie;

using AssociationRegistry.Vereniging;

public record VerwijderLocatieCommand(VCode VCode, int LocatieId);
