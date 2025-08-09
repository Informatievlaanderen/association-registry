namespace AssociationRegistry.DecentraalBeheer.Acties.Contactgegevens.WijzigContactgegevenFromKbo;

using Vereniging;

public record WijzigContactgegevenFromKboCommand(VCode VCode, WijzigContactgegevenFromKboCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(int ContacgegevenId, string? Beschrijving, bool? IsPrimair);
}
