namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.When_Mapping_ReaddressedData;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Schema.Detail;

public class FakeLocatieFinder : ILocatieFinder
{
    private readonly List<LocatieLookupDocument> _locatieLookupDocuments;

    public FakeLocatieFinder(List<LocatieLookupDocument> locatieLookupDocuments)
    {
        _locatieLookupDocuments = locatieLookupDocuments;
    }

    public async Task<IQueryable<LocatieLookupDocument>> FindLocaties(string[] adresIds)
        => _locatieLookupDocuments.Where(x => adresIds.Contains(x.AdresId)).AsQueryable();

    public async Task<LocatieMetVCode[]> FindLocaties(params int[] adresIds)
        => throw new NotImplementedException();
}

public record LocatieLookupMetExpectedAdres(LocatieLookupDocument Document, string ExpectedAdresId);
