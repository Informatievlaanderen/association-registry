namespace AssociationRegistry.Vereniging;

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

        return adres.IsEquivalentTo(otherAdres);
    }
}
