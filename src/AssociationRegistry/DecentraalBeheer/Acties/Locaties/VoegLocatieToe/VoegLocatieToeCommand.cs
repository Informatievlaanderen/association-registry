namespace AssociationRegistry.DecentraalBeheer.Acties.Locaties.VoegLocatieToe;

using Vereniging;

public record VoegLocatieToeCommand(VCode VCode, Locatie Locatie);
