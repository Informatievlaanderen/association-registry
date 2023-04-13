namespace AssociationRegistry.Vereniging.VoegContactgegevenToe;

using Contactgegevens;
using VCodes;

public record VoegContactgegevenToeCommand(VCode VCode, Contactgegeven Contactgegeven)
{
}
