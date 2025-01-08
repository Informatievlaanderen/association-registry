namespace AssociationRegistry.DecentraalBeheer.VerwijderVereniging;

using AssociationRegistry.Vereniging;

public record VerwijderVerenigingCommand(VCode VCode, string Reden);
