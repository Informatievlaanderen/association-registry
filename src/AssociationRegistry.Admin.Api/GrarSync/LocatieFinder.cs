namespace AssociationRegistry.Admin.Api.GrarSync;

using Schema.Detail;

public class LocatieFinder : ILocatieFinder
{
    private readonly List<LocatieLookupDocument> _locatieLookupDocuments;

    public LocatieFinder(List<LocatieLookupDocument> locatieLookupDocuments)
    {
        _locatieLookupDocuments = locatieLookupDocuments;
    }

    public async Task<IEnumerable<LocatieLookupDocument>> FindLocaties(string adresId)
    {
        return _locatieLookupDocuments.Where(x => x.AdresId == adresId);
    }
}
