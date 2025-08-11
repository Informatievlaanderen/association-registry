namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.StopVereniging;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record StopVerenigingCommand(VCode VCode, Datum Einddatum);
