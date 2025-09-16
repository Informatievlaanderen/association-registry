namespace AssociationRegistry.Public.ProjectionHost.Projections.Extensions;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> UpdateSingle<T>(this IEnumerable<T> collection, Func<T, bool> identityFunc, Func<T, T> update)
    {
        var array = collection as T[] ?? collection.ToArray();
        var objectToUpdate = array.Single(identityFunc);
        var updatedObject = update(objectToUpdate);

        return array
              .Where(t1 => !identityFunc(t1))
              .Append(updatedObject);
    }

    public static IEnumerable<T> UpdateSingleWith<T>(this IEnumerable<T> collection, Func<T, bool> identityFunc, Func<T, T> update)
    {
        var array = collection as T[] ?? collection.ToArray();
        var objectToUpdate = array.Single(identityFunc);
        var updatedObject = update(objectToUpdate);

        return array
              .Where(t1 => !identityFunc(t1))
              .Append(updatedObject);
    }
}
