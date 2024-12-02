namespace AssociationRegistry.Admin.Api.GrarConsumer.Finders;

using Grar.GrarUpdates;
using Grar.GrarUpdates.TeHeradresserenLocaties;
using Grar.LocatieFinder;
using Grar.Models;
using Schema.Detail;
using Marten;

public class LocatieFinder : ILocatieFinder
{
    private readonly IDocumentStore _documentStore;

    public LocatieFinder(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<LocatieIdsPerVCodeCollection> FindLocaties(params int[] adresIds)
    {
        return await FindLocaties(
            adresIds
               .Select(x => x.ToString())
               .ToArray());
    }

    public async Task<LocatieIdsPerVCodeCollection> FindLocaties(params string[] adresIds)
    {
        var locatieLookupDocs = await FindLocatieLookupDocuments(adresIds);

        var locatieIdsGroupedByVCode = GroupByVCode(locatieLookupDocs.ToArray());

        return LocatieIdsPerVCodeCollection.FromLocatiesPerVCode(locatieIdsGroupedByVCode);
    }

    public async Task<IEnumerable<LocatieLookupData>> FindLocatieLookupDocuments(string[] adresIds)
    {
        await using var session = _documentStore.LightweightSession();

        return session.Query<LocatieLookupDocument>()
                      .Where(x => adresIds.Contains(x.AdresId))
                      .Select(x => new LocatieLookupData(x.LocatieId, x.AdresId, x.VCode));
    }

    private static Dictionary<string, int[]> GroupByVCode(LocatieLookupData[] locaties)
    {
        return locaties
              .GroupBy(x => x.VCode)
              .ToDictionary(grouping => grouping.Key,
                            documents => documents.Select(x => x.LocatieId)
                                                  .ToArray());
    }
}

