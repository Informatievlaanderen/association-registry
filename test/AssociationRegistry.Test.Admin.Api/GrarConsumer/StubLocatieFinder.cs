namespace AssociationRegistry.Test.Admin.Api.GrarConsumer;

using AssociationRegistry.Grar.GrarUpdates.LocatieFinder;

public class StubLocatieFinder : ILocatieFinder
{
    private readonly Dictionary<string, LocatieLookupData[]> _locatieLookupData;

    public StubLocatieFinder(int sourceAdresId, LocatieLookupData[]? stubData)
    {
        _locatieLookupData = new Dictionary<string, LocatieLookupData[]>()
        {
            { sourceAdresId.ToString(), stubData },
        };
    }

    public async Task<LocatiesPerVCodeCollection> FindLocaties(params string[] adresIds)
        => throw new NotImplementedException();

    public async Task<IEnumerable<LocatieLookupData>> FindLocatieLookupDocuments(string[] adresIds)
        => throw new NotImplementedException();

    public async Task<LocatiesPerVCodeCollection> FindLocaties(params int[] adresIds)
        => await Task.FromResult(LocatiesPerVCodeCollection.FromLocatiesPerVCode(_locatieLookupData));
}
