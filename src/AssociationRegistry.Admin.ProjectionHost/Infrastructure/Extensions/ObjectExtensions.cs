namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;

using System.Collections.Immutable;

public static class ObjectExtensions
{
    public static ImmutableArray<T> ObjectToImmutableArray<T>(this T obj)
        => ImmutableArray.Create(obj);

    public static T[] ObjectToArray<T>(this T obj)
        => new[] { obj };
}
