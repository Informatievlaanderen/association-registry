namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.VerwijderVereniging;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record VerwijderVerenigingCommand(VCode VCode, string Reden);
