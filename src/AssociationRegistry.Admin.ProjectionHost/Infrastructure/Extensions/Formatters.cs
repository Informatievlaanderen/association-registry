namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;

using AssociationRegistry.Events;
using NodaTime;
using Schema;

public static class Formatters
{
    public static string ToAdresString(this  Registratiedata.Locatie locatie)
        => $"{locatie.Straatnaam} {locatie.Huisnummer}" +
           (!string.IsNullOrWhiteSpace(locatie.Busnummer) ? $" bus {locatie.Busnummer}" : string.Empty) +
           $", {locatie.Postcode} {locatie.Gemeente}, {locatie.Land}";

    public static string ToBelgianDate(this Instant instant)
        => instant.ToString(WellknownFormats.DateOnly, WellknownFormats.Belgie);
}
