namespace AssociationRegistry.Vereniging.VoegContactgegevenToe;

using Contactgegevens;

public record VoegContactgegevenToeCommand(string VCode, VoegContactgegevenToeCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(ContactgegevenType Type, string Waarde, string? Omschrijving, bool IsPrimair);
}
