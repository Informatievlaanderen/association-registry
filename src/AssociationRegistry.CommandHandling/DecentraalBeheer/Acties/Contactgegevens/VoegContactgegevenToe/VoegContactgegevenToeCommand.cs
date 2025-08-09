namespace AssociationRegistry.DecentraalBeheer.Acties.Contactgegevens.VoegContactgegevenToe;

using Vereniging;

public record VoegContactgegevenToeCommand(VCode VCode, Contactgegeven Contactgegeven);
