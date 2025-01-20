using System.Globalization;

namespace AssociationRegistry.KboMutations.SyncLambda.JsonSerialization;

public static class DateOnlyHelpers
{
    public static DateOnly TryParse(string dateOnly, string format)
        => DateOnly.TryParseExact(dateOnly, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
            ? result
            : throw new InvalidDateFormat();
}