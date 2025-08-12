namespace AssociationRegistry.Grar.AdresMatch;

using DecentraalBeheer.Vereniging.Adressen;
using DecentraalBeheer.Vereniging.Adressen.GemeentenaamVerrijking;

public record VerrijktAdresUitGrar : Adres
{
    private VerrijktAdresUitGrar(string straatnaam, string huisnummer, string busnummer, string postcode, Gemeentenaam gemeente, string land) : 
        base(straatnaam, huisnummer, busnummer, postcode, gemeente, land)
    {
    }

    public static VerrijktAdresUitGrar FromAdresAndVerrijkteGemeentenaam(Adres adres, VerrijkteGemeentenaam verrijkteGemeentenaam)
        => new(
            straatnaam: adres.Straatnaam,
            huisnummer: adres.Huisnummer,
            busnummer: adres.Busnummer,
            postcode: adres.Postcode,
            gemeente: Gemeentenaam.FromVerrijkteGemeentenaam(verrijkteGemeentenaam),
            Adres.België);
}
