namespace AssociationRegistry.Grar.AdresMatch.Domain;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Vereniging;

public record AdresMatchRequest(
    VCode VCode,
    int LocatieId,
    Locatie? Locatie)
{
    public bool HeeftGeenLocatie => Locatie is null;
    
    public Adres Adres => Locatie?.Adres ?? throw new InvalidOperationException("Cannot access Adres when Locatie is null");
}