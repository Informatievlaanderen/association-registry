namespace AssociationRegistry.Acties.VerwijderVertegenwoordiger;

using Vereniging;

public record VerwijderVertegenwoordigerCommand(VCode VCode, int VertegenwoordigerId);
