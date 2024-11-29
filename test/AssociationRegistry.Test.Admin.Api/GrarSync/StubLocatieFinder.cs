namespace AssociationRegistry.Test.Admin.Api.GrarSync;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Schema.Detail;

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

    public async Task<IQueryable<LocatieLookupDocument>> FindLocaties(params string[] adresIds)
        => throw new NotImplementedException();

    public async Task<LocatieLookupData[]> FindLocaties(params int[] adresIds)
        => adresIds.SelectMany(adresId => _locatieLookupData[adresId.ToString()]).ToArray();
}
