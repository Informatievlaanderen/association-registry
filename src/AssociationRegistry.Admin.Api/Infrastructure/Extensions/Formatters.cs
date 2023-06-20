namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Constants;
using Events;
using NodaTime;

public static class Formatters
{
    public static string ToAdresString(this  Registratiedata.Locatie locatie)
        => $"{locatie.Adres.Straatnaam} {locatie.Adres.Huisnummer}" +
           (!string.IsNullOrWhiteSpace(locatie.Adres.Busnummer) ? $" bus {locatie.Adres.Busnummer}" : string.Empty) +
           $", {locatie.Adres.Postcode} {locatie.Adres.Gemeente}, {locatie.Adres.Land}";

    public static string ToBelgianDate(this Instant instant)
        => instant.ToString(WellknownFormats.DateOnly, WellknownFormats.Belgie);
}
