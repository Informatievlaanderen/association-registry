namespace AssociationRegistry.DecentraalBeheer.Acties.VerwijderVereniging;

using Vereniging;

public record VerwijderVerenigingCommand(VCode VCode, string Reden);
