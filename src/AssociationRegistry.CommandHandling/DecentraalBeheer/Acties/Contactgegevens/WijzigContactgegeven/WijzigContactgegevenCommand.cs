namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.WijzigContactgegeven;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record WijzigContactgegevenCommand(VCode VCode, WijzigContactgegevenCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(int ContacgegevenId, string? Waarde, string? Beschrijving, bool? IsPrimair);
}
