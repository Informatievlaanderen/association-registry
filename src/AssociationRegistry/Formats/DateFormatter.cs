namespace AssociationRegistry.Formats;

using NodaTime;

public static class DateFormatter
{
    private static DateTimeOffset ToBelgianDateTimeOffset(this Instant instant)
    {
        var belgiumTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var utcDateTimeOffset = instant.ToDateTimeOffset();

        return TimeZoneInfo.ConvertTime(utcDateTimeOffset, belgiumTimeZone);
    }


    public static string ToBelgianDateFormat(this Instant instant)
        => instant.ToBelgianDateTimeOffset().ToString(WellknownFormats.DateOnly, WellknownFormats.België);
    public static string ToBelgianTimeFormat(this Instant instant)
        => instant.ToBelgianDateTimeOffset().ToString(WellknownFormats.TimeOnly, WellknownFormats.België);
    public static string ToBelgianDate(this Instant instant)
        => instant.ToString(WellknownFormats.DateOnly, WellknownFormats.België);
    public static string ToBelgianTime(this Instant instant)
        => instant.ToString(WellknownFormats.TimeOnly, WellknownFormats.België);
}
