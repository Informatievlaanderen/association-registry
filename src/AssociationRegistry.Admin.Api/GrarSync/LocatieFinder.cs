namespace AssociationRegistry.Admin.Api.GrarSync;

using Schema.Detail;

public class LocatieFinder : ILocatieFinder
{
    private readonly List<LocatieLookupDocument> _locatieLookupDocuments = new();

    public LocatieFinder()
    {
    }

    public LocatieFinder(List<LocatieLookupDocument> locatieLookupDocuments)
    {
        _locatieLookupDocuments = locatieLookupDocuments;
    }

    public async Task<IEnumerable<LocatieLookupDocument>> FindLocaties(string[] sourceAndDestinationIds)

    {
        return _locatieLookupDocuments.Where(x => sourceAndDestinationIds.Contains(x.AdresId));
    }
}
