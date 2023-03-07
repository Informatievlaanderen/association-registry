namespace AssociationRegistry.Admin.Api.Infrastructure.Json;

using System;
using System.Globalization;

public static class DateOnlyHelpers
{
    public static DateOnly Parse(string dateOnly, string format)
        => DateOnly.TryParseExact(dateOnly, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
            ? result
            : throw new InvalidDateFormat();
}
