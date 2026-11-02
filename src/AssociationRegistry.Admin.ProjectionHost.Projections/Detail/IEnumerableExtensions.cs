namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Schema.Detail;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> UpdateSingle<T>(
        this IEnumerable<T> collection,
        Func<T, bool> identityFunc,
        Func<T, T> update
    )
    {
        var array = collection as T[] ?? collection.ToArray();
        var objectToUpdate = array.Single(identityFunc);
        var updatedObject = update(objectToUpdate);

        return array.Where(t1 => !identityFunc(t1)).Append(updatedObject);
    }

    public static IEnumerable<T> UpdateSingleOrNothing<T>(
        this IEnumerable<T> collection,
        Func<T, bool> identityFunc,
        Func<T, T> update
    )
        where T : IHasBron
    {
        var array = collection as T[] ?? collection.ToArray();
        var objectToUpdate = array.SingleOrDefault(identityFunc);

        if (objectToUpdate is null)
            return array;

        var updatedObject = update(objectToUpdate);

        return array.Where(t1 => !identityFunc(t1)).Append(updatedObject);
    }

    /// <summary>Voegt de items toe; een bestaand item met dezelfde sleutel wordt vervangen.</summary>
    public static IEnumerable<T> AddOrReplace<T, TKey>(
        this IEnumerable<T> collection,
        IEnumerable<T> items,
        Func<T, TKey> keyFunc
    )
    {
        var itemsArray = items as T[] ?? items.ToArray();
        var keys = itemsArray.Select(keyFunc).ToHashSet();

        return collection.Where(item => !keys.Contains(keyFunc(item))).Concat(itemsArray);
    }

    public static IEnumerable<T> UpdateSingleWith<T>(
        this IEnumerable<T> collection,
        Func<T, bool> identityFunc,
        Func<T, T> update
    )
    {
        var array = collection as T[] ?? collection.ToArray();
        var objectToUpdate = array.Single(identityFunc);
        var updatedObject = update(objectToUpdate);

        return array.Where(t1 => !identityFunc(t1)).Append(updatedObject);
    }
}
