namespace AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;

using Normalizers;

public class AdresComparer
{
    private readonly IStringNormalizer _stringNormalizer;

    public AdresComparer(IStringNormalizer stringNormalizer)
    {
        _stringNormalizer = stringNormalizer;
    }

    public bool HasDuplicates(Adres? adres, Adres? otherAdres)
    {
        if (adres is null && otherAdres is null) return false;
        if (adres is null || otherAdres is null) return false;

        return _stringNormalizer.NormalizeString(adres.Straatnaam) == _stringNormalizer.NormalizeString(otherAdres.Straatnaam) &&
               _stringNormalizer.NormalizeString(adres.Postcode) == _stringNormalizer.NormalizeString(otherAdres.Postcode) &&
               _stringNormalizer.NormalizeString(adres.Huisnummer) == _stringNormalizer.NormalizeString(otherAdres.Huisnummer) &&
               _stringNormalizer.NormalizeString(adres.Busnummer) == _stringNormalizer.NormalizeString(otherAdres.Busnummer) &&
               _stringNormalizer.NormalizeString(adres.Gemeente.Naam) == _stringNormalizer.NormalizeString(otherAdres.Gemeente.Naam) &&
               _stringNormalizer.NormalizeString(adres.Land) == _stringNormalizer.NormalizeString(otherAdres.Land);
    }
}
