namespace AssociationRegistry.Acties.VoegContactgegevenToe;

using Vereniging;

public record VoegContactgegevenToeCommand(VCode VCode, Contactgegeven Contactgegeven)
{
}
