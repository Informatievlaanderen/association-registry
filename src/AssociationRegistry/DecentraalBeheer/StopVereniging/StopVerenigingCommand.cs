namespace AssociationRegistry.DecentraalBeheer.StopVereniging;

using AssociationRegistry.Vereniging;

public record StopVerenigingCommand(VCode VCode, Datum Einddatum);
