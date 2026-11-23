namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

/// <summary>The allowed values of a value object in a message: "A, B of C".</summary>
internal static class AllowedValues
{
    public static string Describe(IEnumerable<string> values)
    {
        var all = values.ToArray();

        return all.Length == 1 ? all[0] : $"{string.Join(", ", all[..^1])} of {all[^1]}";
    }
}
