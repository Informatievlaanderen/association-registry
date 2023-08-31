namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Schema.Detail;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> UpdateSingle<T>(this IEnumerable<T> collection, Func<T, bool> identityFunc, Func<T, T> update) where T : IHasBron
    {
        var array = collection as T[] ?? collection.ToArray();
        var objectToUpdate = array.Single(identityFunc);
        var updatedObject = update(objectToUpdate);
        return array
              .Where(t1 => !identityFunc(t1))
              .Append(updatedObject);
    }
}
