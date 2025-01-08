namespace AssociationRegistry.DecentraalBeheer.Contactgegevens.WijzigContactgegeven;

using AssociationRegistry.Vereniging;

public record WijzigContactgegevenCommand(VCode VCode, WijzigContactgegevenCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(int ContacgegevenId, string? Waarde, string? Beschrijving, bool? IsPrimair);
}
