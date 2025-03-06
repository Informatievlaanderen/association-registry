namespace AssociationRegistry.Test.Framework.Helpers;

using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Repeat<T>(this IEnumerable<T> collection, int times)
        => Enumerable.Repeat(collection.ToList(), times).SelectMany(_ => _);
}
