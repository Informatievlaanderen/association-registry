namespace AssociationRegistry.Test.Public.Api.Framework;

public class CaseSensitiveComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
        => string.Compare(x, y, StringComparison.Ordinal);
}
