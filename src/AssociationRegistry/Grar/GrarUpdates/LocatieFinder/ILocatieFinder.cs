namespace AssociationRegistry.Grar.GrarUpdates.LocatieFinder;

public interface ILocatieFinder
{
    Task<LocatieIdsPerVCodeCollection> FindLocaties(params int[] adresIds);
    Task<LocatieIdsPerVCodeCollection> FindLocaties(params string[] adresIds);
    Task<IEnumerable<LocatieLookupData>> FindLocatieLookupDocuments(string[] adresIds);
}

