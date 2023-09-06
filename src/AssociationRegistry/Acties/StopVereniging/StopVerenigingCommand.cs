namespace AssociationRegistry.Acties.StopVereniging;

using Vereniging;

public record StopVerenigingCommand(VCode VCode, Datum Einddatum);
