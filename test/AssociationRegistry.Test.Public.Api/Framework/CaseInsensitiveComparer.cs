namespace AssociationRegistry.Test.Public.Api.Framework;

public class CaseInsensitiveComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
        => string.Compare(x?.Trim(), y?.Trim(), StringComparison.InvariantCultureIgnoreCase);
}
