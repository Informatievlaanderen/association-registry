namespace AssociationRegistry.Extensions;

public static class StringArrayExtensions
{
    public static bool IsNullOrEmpty(this string[]? values) => values is null || values.Length == 0;

    public static bool HasNullOrWhiteSpaceValues(this string[] values) => values.Any(string.IsNullOrWhiteSpace);

    public static bool HasCaseInsensitiveDuplicateValues(this string[] values) =>
        values.Distinct(StringComparer.OrdinalIgnoreCase).Count() != values.Length;
}
