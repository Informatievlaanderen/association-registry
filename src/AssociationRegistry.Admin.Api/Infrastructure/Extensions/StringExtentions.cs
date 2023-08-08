namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using System;

public static class StringExtentions
{
    public static bool IsDateBeforeToday(this string datumString)
        => string.IsNullOrWhiteSpace(datumString) ||
           (DateOnly.TryParseExact(datumString, "yyyy-MM-dd", out var date) &&
            date <= DateOnly.FromDateTime(DateTime.Now));

    public static bool IsDateAfterToday(this string datumString)
        => string.IsNullOrWhiteSpace(datumString) ||
           (DateOnly.TryParseExact(datumString, "yyyy-MM-dd", out var date) &&
            date >= DateOnly.FromDateTime(DateTime.Now));
}
