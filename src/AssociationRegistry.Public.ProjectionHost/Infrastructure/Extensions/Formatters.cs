namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;

using Constants;
using NodaTime;

public static class Formatters
{
    public static string ToBelgianDate(this Instant instant)
        => instant.ToString(WellknownFormats.DateOnly, WellknownFormats.België);
    public static string ToBelgianDate(this DateOnly? dateOnly)
        => dateOnly?.ToString(WellknownFormats.DateOnly, WellknownFormats.België) ??
           string.Empty;
}
