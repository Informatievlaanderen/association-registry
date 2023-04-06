namespace AssociationRegistry.Vereniging.WijzigContactgegeven;

public record WijzigContactgegevenCommand(string VCode, WijzigContactgegevenCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(int ContacgegevenId, string? Waarde, string? Omschrijving, bool? IsPrimair);
}
