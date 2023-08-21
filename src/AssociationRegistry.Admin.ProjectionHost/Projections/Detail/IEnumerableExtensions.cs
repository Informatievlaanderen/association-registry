namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Schema.Detail;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> Replace<T>(this IEnumerable<T> collection, Func<T, bool> identityFunc, T entity) where T : IHasBron
    {
        var array = collection as T[] ?? collection.ToArray();
        entity.Bron = array.Single(identityFunc).Bron;
        return array
            .Where(t1 => !identityFunc(t1))
            .Append(entity);
    }
}
