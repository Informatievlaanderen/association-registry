namespace AssociationRegistry.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;

using Vereniging;

public record VerwijderVertegenwoordigerCommand(VCode VCode, int VertegenwoordigerId);
