namespace AssociationRegistry.Admin.AddressSync;

using AssociationRegistry.Grar.Integration.Messages;
using Grar;
using Grar.Clients;
using Grar.Exceptions;
using Grar.Models;
using Marten;
using Microsoft.Extensions.Logging;
using Schema.Detail;
using System.Diagnostics.Contracts;

public class TeSynchroniserenLocatiesFetcher(
    IGrarClient grarClient,
    ILogger<TeSynchroniserenLocatiesFetcher> logger)
    : ITeSynchroniserenLocatiesFetcher
{
    public async Task<IEnumerable<TeSynchroniserenLocatieAdresMessage>> GetTeSynchroniserenLocaties(
        IDocumentSession session,
        CancellationToken stoppingToken)
    {
        logger.LogInformation("Fetcher started.");

        var locatieLookupDocuments = await session.Query<LocatieLookupDocument>().ToListAsync(stoppingToken);

        logger.LogInformation("Fetcher found {DocumentCount}.", locatieLookupDocuments.Count);

        var idempotenceKey = Guid.NewGuid().ToString("N");

        var messages = GroupByVCode(locatieLookupDocuments,
                                    await GetDetailsFromAdressenregister(GroupByAddressId(locatieLookupDocuments), stoppingToken))
           .Select(x => new TeSynchroniserenLocatieAdresMessage(x.VCode, x.LocatieWithAdres, idempotenceKey));

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
                                                                                                addressResults.FirstOrDefault(
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
            try
            {
                var response = await grarClient.GetAddressById(resultByAdresId.AdresId, cancellationToken);
                addressResults.Add(response);

                logger.LogInformation("Adres ontvangen voor adres ID {AdresId}: {Adres}", resultByAdresId.AdresId,
                                      response.Adresvoorstelling);
            }
            catch (AdressenregisterReturnedGoneStatusCode ex)
            {
                logger.LogError(ex, "Adres met ID {AdresId} is verwijderd uit het adressenregister.", resultByAdresId.AdresId);
            }
            catch (AdressenregisterReturnedNotFoundStatusCode ex)
            {
                logger.LogError(ex, "Adres met ID {AdresId} werd niet gevonden in het adressenregister.", resultByAdresId.AdresId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Adres kon niet opgehaald worden voor ID {AdresId}.", resultByAdresId.AdresId);

                throw;
            }
        }

        return addressResults;
    }
}
