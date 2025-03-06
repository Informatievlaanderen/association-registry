namespace AssociationRegistry.Test.Admin.Api.GrarConsumer;

using Grar.GrarUpdates.LocatieFinder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
