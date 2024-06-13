namespace AssociationRegistry.Admin.Api.GrarSync;

using Marten;
using Schema.Detail;

public class LocatieFinder : ILocatieFinder
{
    private readonly IDocumentStore _documentStore;

    public LocatieFinder(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<IEnumerable<LocatieLookupDocument>> FindLocaties(string[] sourceAndDestinationIds)

    {
        await using var session = _documentStore.LightweightSession();

        return session.Query<LocatieLookupDocument>().Where(x => sourceAndDestinationIds.Contains(x.AdresId));
    }
}
