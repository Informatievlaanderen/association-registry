namespace AssociationRegistry.Grar.GrarUpdates.LocatieFinder;

public interface ILocatieFinder
{
    Task<LocatiesPerVCodeCollection> FindLocaties(params int[] adresIds);
    Task<LocatiesPerVCodeCollection> FindLocaties(params string[] adresIds);
    Task<IEnumerable<LocatieLookupData>> FindLocatieLookupDocuments(string[] adresIds);
}

