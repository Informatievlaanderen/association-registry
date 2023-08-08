namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using System;

public static class DateOnlyHelper
{
    public static DateOnly? ParseOrNull(string? datumString, string format)
    {
        if (string.IsNullOrWhiteSpace(datumString))
            return null;
        return DateOnly.TryParseExact(datumString, format, out var date)
            ? date
            : null;
    }
}
