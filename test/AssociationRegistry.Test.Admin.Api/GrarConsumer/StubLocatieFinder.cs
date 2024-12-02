namespace AssociationRegistry.Test.Admin.Api.GrarConsumer;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Schema.Detail;
using Grar.GrarUpdates;
using Grar.GrarUpdates.LocatieFinder;

public class StubLocatieFinder : ILocatieFinder
{
    private readonly Dictionary<string, int[]> _locatieLookupData;

    public StubLocatieFinder(int sourceAdresId, int[]? stubData)
    {
        _locatieLookupData = new Dictionary<string, int[]>()
        {
            { sourceAdresId.ToString(), stubData },
        };
    }

    public async Task<LocatieIdsPerVCodeCollection> FindLocaties(params string[] adresIds)
        => throw new NotImplementedException();

    public async Task<IEnumerable<LocatieLookupData>> FindLocatieLookupDocuments(string[] adresIds)
        => throw new NotImplementedException();

    public async Task<LocatieIdsPerVCodeCollection> FindLocaties(params int[] adresIds)
        => await Task.FromResult(LocatieIdsPerVCodeCollection.FromLocatiesPerVCode(_locatieLookupData));
}
