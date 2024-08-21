namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Mapping_ReaddressedData;

using AssociationRegistry.Admin.Api.GrarSync;
using AssociationRegistry.Admin.Schema.Detail;

public class FakeLocatieFinder : ILocatieFinder
{
    private readonly List<LocatieLookupDocument> _locatieLookupDocuments;

    public FakeLocatieFinder(List<LocatieLookupDocument> locatieLookupDocuments)
    {
        _locatieLookupDocuments = locatieLookupDocuments;
    }

    public async Task<IEnumerable<LocatieLookupDocument>> FindLocaties(string[] adresIds)
        => _locatieLookupDocuments.Where(x => adresIds.Contains(x.AdresId));
}

public record LocatieLookupMetExpectedAdres(LocatieLookupDocument Document, string ExpectedAdresId);
