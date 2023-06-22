namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using AssociationRegistry.Formatters;
using Constants;
using Events;
using NodaTime;

public static class Formatters
{
    public static string ToBelgianDate(this Instant instant)
        => instant.ToString(WellknownFormats.DateOnly, WellknownFormats.Belgie);
}
