namespace AssociationRegistry.DecentraalBeheer.Contactgegevens.VoegContactgegevenToe;

using AssociationRegistry.Vereniging;

public record VoegContactgegevenToeCommand(VCode VCode, Contactgegeven Contactgegeven);
