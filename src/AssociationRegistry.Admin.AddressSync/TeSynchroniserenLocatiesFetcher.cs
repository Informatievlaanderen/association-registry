namespace AssociationRegistry.Admin.AddressSync;

using Grar;
using Grar.AddressSync;
using Grar.Models;
using Marten;
using Schema.Detail;
using System.Diagnostics.Contracts;

public class TeSynchroniserenLocatiesFetcher(IGrarClient grarClient)
    : ITeSynchroniserenLocatiesFetcher
{
    public async Task<IEnumerable<SynchroniseerLocatieMessage>> GetTeSynchroniserenLocaties(
        IDocumentSession session,
        CancellationToken stoppingToken)
    {
        var locatieLookupDocuments = await session.Query<LocatieLookupDocument>().ToListAsync(stoppingToken);
        var idempotenceKey = Guid.NewGuid().ToString("N");

        var messages = GroupByVCode(locatieLookupDocuments,
                                    await GetDetailsFromAdressenregister(GroupByAddressId(locatieLookupDocuments), stoppingToken))
           .Select(x => new SynchroniseerLocatieMessage(x.VCode, x.LocatieWithAdres, idempotenceKey));

        return messages;
    }

    [Pure]
    private static NachtelijkeAdresSyncVolgensVCode[] GroupByVCode(
        IReadOnlyList<LocatieLookupDocument> locatieLookupDocuments,
        HashSet<AddressDetailResponse> addressResults)
        => locatieLookupDocuments.GroupBy(g => g.VCode)
                                 .Select(s => new NachtelijkeAdresSyncVolgensVCode(s.Key,
                                                                                   s.Select(x => new LocatieWithAdres(
                                                                                                x.LocatieId,
                                                                                                addressResults.Single(
                                                                                                    r => r.AdresId.Bronwaarde.Split('/')
                                                                                                       .Last() == x.AdresId)))
                                                                                    .ToList()))
                                 .ToArray();

    [Pure]
    private static NachtelijkeAdresSyncVolgensAdresId[] GroupByAddressId(IReadOnlyList<LocatieLookupDocument> locatieLookupDocuments)
        => locatieLookupDocuments.GroupBy(g => g.AdresId)
                                 .Select(s => new NachtelijkeAdresSyncVolgensAdresId(s.Key,
                                                                                     s.Select(x => new LocatieIdWithVCode(
                                                                                         x.LocatieId, x.VCode)).ToList()))
                                 .ToArray();

    private async Task<HashSet<AddressDetailResponse>> GetDetailsFromAdressenregister(
        NachtelijkeAdresSyncVolgensAdresId[] byAdresId,
        CancellationToken cancellationToken)
    {
        var addressResults = new HashSet<AddressDetailResponse>();

        foreach (var resultByAdresId in byAdresId)
        {
            var response = await grarClient.GetAddressById(resultByAdresId.AdresId, cancellationToken);
            addressResults.Add(response);
        }

        return addressResults;
    }
}
