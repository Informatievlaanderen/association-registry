namespace AssociationRegistry.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;

using AssociationRegistry.Vereniging;

public record VerwijderVertegenwoordigerCommand(VCode VCode, int VertegenwoordigerId);
