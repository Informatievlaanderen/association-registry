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

    public async Task<LocatieMetVCode[]> FindLocaties(params int[] adresIds)
    {
        var locaties = await FindLocaties(adresIds.Select(x => x.ToString()).ToArray());

        return locaties.Select(s => new LocatieMetVCode(s.VCode, s.LocatieId)).ToArray();
    }
}
