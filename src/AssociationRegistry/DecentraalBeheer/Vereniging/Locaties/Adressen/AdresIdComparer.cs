namespace AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;

public class AdresIdComparer
{
    public bool HasDuplicates(AdresId? adresId, AdresId? otherAdresId)
    {
        if (adresId is null && otherAdresId is null) return false;

        return adresId == otherAdresId;
    }
}
