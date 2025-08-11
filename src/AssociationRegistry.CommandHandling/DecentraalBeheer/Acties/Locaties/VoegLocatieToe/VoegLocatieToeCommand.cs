namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.VoegLocatieToe;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record VoegLocatieToeCommand(VCode VCode, Locatie Locatie);
