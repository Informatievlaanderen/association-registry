namespace AssociationRegistry.Formats;

using NodaTime;

public static class DateFormatter
{
    public static string ToBelgianDate(this Instant instant)
        => instant.ToString(WellknownFormats.DateOnly, WellknownFormats.België);
    public static string ToBelgianDate(this LocalDateTime instant)
        => instant.ToString(WellknownFormats.DateOnly, WellknownFormats.België);

    public static string ToBelgianDate(this DateOnly instant)
        => instant.ToString(WellknownFormats.DateOnly, WellknownFormats.België);
}
