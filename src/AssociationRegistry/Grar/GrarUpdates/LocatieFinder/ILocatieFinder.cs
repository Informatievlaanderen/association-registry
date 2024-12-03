namespace AssociationRegistry.Grar.GrarUpdates.LocatieFinder;

public interface ILocatieFinder
{
    Task<LocatiesPerVCodeCollection> FindLocaties(params int[] adresIds);
    Task<LocatiesPerVCodeCollection> FindLocaties(params string[] adresIds);
}

