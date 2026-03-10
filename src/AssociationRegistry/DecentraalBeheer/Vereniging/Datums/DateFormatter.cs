namespace AssociationRegistry.Formats;

using NodaTime;
using NodaTime.Text;

public static class DateFormatter
{
    private static DateTimeOffset ToBelgianDateTimeOffset(this Instant instant)
    {
        var belgiumTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var utcDateTimeOffset = instant.ToDateTimeOffset();

        return TimeZoneInfo.ConvertTime(utcDateTimeOffset, belgiumTimeZone);
    }

    public static string ConvertAndFormatToBelgianDate(this Instant instant) =>
        instant.ToBelgianDateTimeOffset().FormatAsBelgianDate();

    public static string ConvertAndFormatToBelgianTime(this Instant instant) =>
        instant.ToBelgianDateTimeOffset().FormatAsBelgianTime();

    public static string FormatAsBelgianDate(this Instant instant) =>
        instant.ToString(WellknownFormats.DateOnly, WellknownFormats.België);

    public static string FormatAsBelgianDate(this DateOnly dateOnly) =>
        dateOnly.ToString(WellknownFormats.DateOnly, WellknownFormats.België);

    public static string FormatAsBelgianDate(this DateOnly? dateOnly) =>
        dateOnly?.ToString(WellknownFormats.DateOnly, WellknownFormats.België) ?? string.Empty;

    public static string FormatAsBelgianTime(this Instant instant) =>
        instant.ToString(WellknownFormats.TimeOnly, WellknownFormats.België);

    public static string FormatAsBelgianDate(this DateTimeOffset instant) =>
        instant.ToString(WellknownFormats.DateOnly, WellknownFormats.België);

    public static string FormatAsBelgianTime(this DateTimeOffset instant) =>
        instant.ToString(WellknownFormats.TimeOnly, WellknownFormats.België);

    public static string FormatAsZuluTime(this Instant instant) =>
        instant.ToDateTimeUtc().ToString("yyyy-MM-ddTHH:mm:ssZ");

    public static Instant ToInstant(this DateTimeOffset dateTimeOffset)
    {
        return Instant.FromDateTimeOffset(dateTimeOffset);
    }

    public static Instant ToInstant(this string value)
    {
        return InstantPattern.ExtendedIso.Parse(value).Value;
    }
}
