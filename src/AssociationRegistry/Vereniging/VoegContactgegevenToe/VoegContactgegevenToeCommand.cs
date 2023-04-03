namespace AssociationRegistry.Vereniging.VoegContactgegevenToe;

using ContactGegevens;

public record VoegContactgegevenToeCommand(string VCode, VoegContactgegevenToeCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(ContactgegevenType Type, string Waarde, string? Omschrijving, bool IsPrimair);
}
