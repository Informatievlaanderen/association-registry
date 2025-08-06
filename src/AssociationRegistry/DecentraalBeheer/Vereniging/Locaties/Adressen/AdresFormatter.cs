namespace AssociationRegistry.Formats;

using Events;
using DecentraalBeheer.Vereniging.Adressen;

public static class AdresFormatter
{
    public static string ToAdresString(this Registratiedata.Adres? adres)
    {
        if (adres is null)
            return string.Empty;

        return $"{adres.Straatnaam} {adres.Huisnummer}" +
               (!string.IsNullOrWhiteSpace(adres.Busnummer) ? $" bus {adres.Busnummer}" : string.Empty) +
               $", {adres.Postcode} {adres.Gemeente}, {adres.Land}";
    }

    public static string ToAdresString(this Registratiedata.AdresUitAdressenregister? adres)
    {
        if (adres is null)
            return string.Empty;

        return $"{adres.Straatnaam} {adres.Huisnummer}" +
               (!string.IsNullOrWhiteSpace(adres.Busnummer) ? $" bus {adres.Busnummer}" : string.Empty) +
               $", {adres.Postcode} {adres.Gemeente}, {Adres.België}";
    }
}
