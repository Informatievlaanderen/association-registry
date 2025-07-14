namespace AssociationRegistry.Shared.Serialization.JsonConverters;

using System.Globalization;
using Vereniging.Exceptions;

public static class DateOnlyHelpers
{
    public static DateOnly TryParseExactOrThrow(string dateOnly, string format)
        => DateOnly.TryParseExact(dateOnly, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
            ? result
            : throw new DatumFormaatIsOngeldig();
}
