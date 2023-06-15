namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;

using AssociationRegistry.Events;
using Constants;
using NodaTime;

public static class Formatters
{
    public static string ToAdresString(this  Registratiedata.Locatie locatie)
        => $"{locatie.Straatnaam} {locatie.Huisnummer}" +
           (!string.IsNullOrWhiteSpace(locatie.Busnummer) ? $" bus {locatie.Busnummer}" : string.Empty) +
           $", {locatie.Postcode} {locatie.Gemeente}, {locatie.Land}";

    public static string ToBelgianDate(this Instant instant)
        => instant.ToString(WellknownFormats.DateOnly, WellknownFormats.Belgie);

    public static string ToBelgianDateAndTime(this Instant instant)
        => instant.ToString(WellknownFormats.DateAndTime, WellknownFormats.Belgie);
}
