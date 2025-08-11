namespace AssociationRegistry.Grar.AdresMatch;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;

public record AdresMatchRequest(Locatie? Locatie)
{
    public bool HeeftGeenLocatie => Locatie is null;

    public Adres Adres => Locatie?.Adres ?? throw new InvalidOperationException("Cannot access Adres when Locatie is null");
}
