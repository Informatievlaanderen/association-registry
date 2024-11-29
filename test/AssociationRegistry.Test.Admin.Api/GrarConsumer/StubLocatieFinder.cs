namespace AssociationRegistry.Test.Admin.Api.GrarConsumer;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Schema.Detail;

public class StubLocatieFinder : ILocatieFinder
{
    private readonly Dictionary<string, LocatieMetVCode[]> _locatieLookupData;

    public StubLocatieFinder(int sourceAdresId, LocatieMetVCode[]? stubData)
    {
        _locatieLookupData = new Dictionary<string, LocatieMetVCode[]>()
        {
            { sourceAdresId.ToString(), stubData },
        };
    }

    public async Task<IQueryable<LocatieLookupDocument>> FindLocaties(params string[] adresIds)
        => throw new NotImplementedException();

    public async Task<LocatieMetVCode[]> FindLocaties(params int[] adresIds)
        => adresIds.SelectMany(adresId => _locatieLookupData[adresId.ToString()]).ToArray();
}
