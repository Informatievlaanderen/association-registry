namespace AssociationRegistry.Test.Common.Extensions.Mappers;

public static class AdresToAdresStringExtensions
{
    public static string ToAdresString(this AssociationRegistry.Admin.Api.Verenigingen.Common.Adres? adres)
    {
        if (adres is null)
            return string.Empty;

        return $"{adres.Straatnaam} {adres.Huisnummer}" +
               (!string.IsNullOrWhiteSpace(adres.Busnummer) ? $" bus {adres.Busnummer}" : string.Empty) +
               $", {adres.Postcode} {adres.Gemeente}, {adres.Land}";
    }
}
