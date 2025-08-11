namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.WijzigContactgegevenFromKbo;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record WijzigContactgegevenFromKboCommand(VCode VCode, WijzigContactgegevenFromKboCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(int ContacgegevenId, string? Beschrijving, bool? IsPrimair);
}
