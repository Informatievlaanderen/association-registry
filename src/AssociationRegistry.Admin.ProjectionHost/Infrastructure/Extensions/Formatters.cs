namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;

using Events;
using Constants;
using NodaTime;

public static class Formatters
{
    public static string ToAdresString(this  Registratiedata.Locatie locatie)
        => $"{locatie.Adres.Straatnaam} {locatie.Adres.Huisnummer}" +
           (!string.IsNullOrWhiteSpace(locatie.Adres.Busnummer) ? $" bus {locatie.Adres.Busnummer}" : string.Empty) +
           $", {locatie.Adres.Postcode} {locatie.Adres.Gemeente}, {locatie.Adres.Land}";

    public static string ToBelgianDate(this Instant instant)
        => instant.ToString(WellknownFormats.DateOnly, WellknownFormats.Belgie);

    public static string ToBelgianDateAndTime(this Instant instant)
        => instant.ToString(WellknownFormats.DateAndTime, WellknownFormats.Belgie);
}
