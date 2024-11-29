namespace AssociationRegistry.Admin.Api.GrarConsumer.Finders;

using Schema.Detail;
using Marten;

public class LocatieFinder : ILocatieFinder
{
    private readonly IDocumentStore _documentStore;

    public LocatieFinder(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<IQueryable<LocatieLookupDocument>> FindLocaties(string[] adresIds)
    {
        await using var session = _documentStore.LightweightSession();

        return session.Query<LocatieLookupDocument>()
                      .Where(x => adresIds.Contains(x.AdresId));
    }

    public async Task<LocatieIdsPerVCode> FindLocaties(params int[] adresIds)
    {
        var locaties = await FindLocaties(adresIds.Select(x => x.ToString()).ToArray());

        return LocatieIdsPerVCode.FromLocatieLookupDocuments(locaties.ToArray());
    }
}

public class LocatieIdsPerVCode : Dictionary<string, int[]>
{
    public LocatieIdsPerVCode(Dictionary<string, int[]> dictionary): base(dictionary)
    {

    }
    public static LocatieIdsPerVCode FromLocatieLookupDocuments(IEnumerable<LocatieLookupDocument> locaties)
    {
        return new(locaties.GroupBy(x => x.VCode)
                           .ToDictionary(grouping => grouping.Key,
                                         documents => documents.Select(x => x.LocatieId).ToArray()));
    }
}
