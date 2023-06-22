namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;

using Constants;
using NodaTime;

public static class Formatters
{
    public static string ToBelgianDate(this Instant instant)
        => instant.ToString(WellknownFormats.DateOnly, WellknownFormats.Belgie);

    public static string ToBelgianDateAndTime(this Instant instant)
        => instant.ToString(WellknownFormats.DateAndTime, WellknownFormats.Belgie);
}
