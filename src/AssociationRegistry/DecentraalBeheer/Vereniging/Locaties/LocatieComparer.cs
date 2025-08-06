namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Adressen;
using Normalizers;

public class LocatieComparer
{
    private readonly AdresComparer _adresComparer;
    private readonly AdresIdComparer _adresIdComparer;

    public LocatieComparer()
    {
        _adresComparer = new AdresComparer(new AdresComponentNormalizer());
        _adresIdComparer = new AdresIdComparer();
    }

    public bool HasDuplicates(IEnumerable<Locatie> locaties, Locatie otherLocatie)
    {
        return locaties.Any(locatie => IsADuplicateOf(locatie, otherLocatie));
    }

    private bool IsADuplicateOf(Locatie locatie, Locatie otherLocatie)
        => locatie.Naam == otherLocatie.Naam &&
           locatie.Locatietype == otherLocatie.Locatietype &&
           (_adresComparer.HasDuplicates(locatie.Adres, otherLocatie.Adres) ||
            _adresIdComparer.HasDuplicates(locatie.AdresId, otherLocatie.AdresId));
}
