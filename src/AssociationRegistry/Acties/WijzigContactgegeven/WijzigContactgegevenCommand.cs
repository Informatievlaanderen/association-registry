namespace AssociationRegistry.Acties.WijzigContactgegeven;

using Vereniging;

public record WijzigContactgegevenCommand(VCode VCode, WijzigContactgegevenCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(int ContacgegevenId, string? Waarde, string? Omschrijving, bool? IsPrimair);
}
