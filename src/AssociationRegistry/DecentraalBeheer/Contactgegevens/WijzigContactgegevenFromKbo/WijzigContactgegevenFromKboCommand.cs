namespace AssociationRegistry.DecentraalBeheer.Contactgegevens.WijzigContactgegevenFromKbo;

using AssociationRegistry.Vereniging;

public record WijzigContactgegevenFromKboCommand(VCode VCode, WijzigContactgegevenFromKboCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(int ContacgegevenId, string? Beschrijving, bool? IsPrimair);
}
