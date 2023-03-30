namespace AssociationRegistry.Vereniging.AddContactgegevens;

public record VoegContactgegevenToeCommand(string VCode, VoegContactgegevenToeCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(string Type, string Waarde, string? Omschrijving, bool IsPrimair);
}
