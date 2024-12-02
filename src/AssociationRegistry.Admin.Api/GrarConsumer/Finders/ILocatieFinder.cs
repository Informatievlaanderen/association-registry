namespace AssociationRegistry.Grar.GrarUpdates;

public interface ILocatieFinder
{
    Task<IQueryable<LocatieLookupDocument>> FindLocaties(params string[] adresIds);
    Task<LocatieIdsPerVCodeCollection> FindLocaties(params int[] adresIds);
}
