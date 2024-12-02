namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.ReAddressEvents.When_Mapping_ReaddressedData;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Schema.Detail;
using Grar.GrarUpdates;
using Grar.GrarUpdates.LocatieFinder;

public class FakeLocatieFinder : ILocatieFinder
{
    private readonly List<LocatieLookupDocument> _locatieLookupDocuments;

    public FakeLocatieFinder(List<LocatieLookupDocument> locatieLookupDocuments)
    {
        _locatieLookupDocuments = locatieLookupDocuments;
    }

    public async Task<IEnumerable<LocatieLookupData>> FindLocatieLookupDocuments(string[] adresIds)
        => _locatieLookupDocuments.Where(x => adresIds.Contains(x.AdresId))
                                  .Select(x => new LocatieLookupData(x.LocatieId, x.AdresId, x.VCode))
                                  .ToList();

    public async Task<LocatiesPerVCodeCollection> FindLocaties(params int[] adresIds)
        => throw new NotImplementedException();

    async Task<LocatiesPerVCodeCollection> ILocatieFinder.FindLocaties(params string[] adresIds)
        => throw new NotImplementedException();
}

public record LocatieLookupMetExpectedAdres(LocatieLookupDocument Document, string ExpectedAdresId);
