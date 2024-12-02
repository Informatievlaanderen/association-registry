namespace AssociationRegistry.Admin.Api.GrarConsumer.Finders;

using Grar.GrarUpdates;
using Grar.GrarUpdates.TeHeradresserenLocaties;
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

    public async Task<IQueryable<LocatieLookupDocument>> FindLocaties(string[] adresIds)
    {
        await using var session = _documentStore.LightweightSession();

        return session.Query<LocatieLookupDocument>()
                      .Where(x => adresIds.Contains(x.AdresId));
    }

    public async Task<LocatieIdsPerVCodeCollection> FindLocaties(params int[] adresIds)
    {
        var locaties = await FindLocaties(adresIds.Select(x => x.ToString()).ToArray());

        return LocatieIdsPerVCodeCollection.FromLocatieLookupDocuments(locaties.ToArray());
    }
}

public record LocatieIdsPerVCode(string VCode, int[] LocatieIds);

public class LocatieIdsPerVCodeCollection : List<LocatieIdsPerVCode>
{
    private LocatieIdsPerVCodeCollection(Dictionary<string, int[]> dictionary)
        : base(dictionary.Select(s => new LocatieIdsPerVCode(s.Key, dictionary[s.Key])))
    {
    }

    public static LocatieIdsPerVCodeCollection FromLocatieLookupDocuments(IEnumerable<LocatieLookupDocument> locaties)
    {
        return new(locaties.GroupBy(x => x.VCode)
                           .ToDictionary(grouping => grouping.Key,
                                         documents => documents.Select(x => x.LocatieId).ToArray()));
    }

    public static IEnumerable<TeHeradresserenLocatiesMessage> Map(LocatieIdsPerVCodeCollection locatieIdsPerVCodeCollection, int destinationAdresId)
    {
        return locatieIdsPerVCodeCollection.Select(x => new TeHeradresserenLocatiesMessage(
                                             x.VCode,
                                             x.LocatieIds.Select(locatieId => new TeHeradresserenLocatie(locatieId, destinationAdresId.ToString()))
                                              .ToList(),
                                             ""));
    }
}
