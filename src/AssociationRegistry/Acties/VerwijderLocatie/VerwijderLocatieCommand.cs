namespace AssociationRegistry.Acties.VerwijderLocatie;

using Vereniging;

public record VerwijderLocatieCommand(VCode VCode, int LocatieId);
