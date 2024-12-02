namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.ReAddressEvents.When_Mapping_ReaddressedData;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Schema.Detail;
using Grar.GrarUpdates;

public class FakeLocatieFinder : ILocatieFinder
{
    private readonly List<LocatieLookupDocument> _locatieLookupDocuments;

    public FakeLocatieFinder(List<LocatieLookupDocument> locatieLookupDocuments)
    {
        _locatieLookupDocuments = locatieLookupDocuments;
    }

    public async Task<IQueryable<LocatieLookupDocument>> FindLocaties(string[] adresIds)
        => _locatieLookupDocuments.Where(x => adresIds.Contains(x.AdresId)).AsQueryable();

    public async Task<LocatieIdsPerVCodeCollection> FindLocaties(params int[] adresIds)
        => throw new NotImplementedException();
}

public record LocatieLookupMetExpectedAdres(LocatieLookupDocument Document, string ExpectedAdresId);
