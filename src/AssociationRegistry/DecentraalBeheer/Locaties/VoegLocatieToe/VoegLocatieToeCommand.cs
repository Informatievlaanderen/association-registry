namespace AssociationRegistry.DecentraalBeheer.Locaties.VoegLocatieToe;

using AssociationRegistry.Vereniging;

public record VoegLocatieToeCommand(VCode VCode, Locatie Locatie);
