namespace AssociationRegistry.Test.Admin.Api.Framework;

using System;
using System.Collections.Generic;

public class CaseInsensitiveComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
        => string.Compare(x?.Trim(), y?.Trim(), StringComparison.OrdinalIgnoreCase);
}

public class CaseSensitiveComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
        => string.Compare(x, y, StringComparison.Ordinal);
}
