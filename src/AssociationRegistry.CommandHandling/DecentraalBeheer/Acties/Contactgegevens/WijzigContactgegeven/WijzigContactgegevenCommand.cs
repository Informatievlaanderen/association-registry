namespace AssociationRegistry.DecentraalBeheer.Acties.Contactgegevens.WijzigContactgegeven;

using Vereniging;

public record WijzigContactgegevenCommand(VCode VCode, WijzigContactgegevenCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(int ContacgegevenId, string? Waarde, string? Beschrijving, bool? IsPrimair);
}
