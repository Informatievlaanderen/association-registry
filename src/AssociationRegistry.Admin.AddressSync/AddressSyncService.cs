namespace AssociationRegistry.Admin.AddressSync;

using Grar;
using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Schema.Detail;

public record NachtelijkeAdresSync(string AdresId, List<LocatieIdWithVCode> LocatieIdWithVCodes);
public record LocatieIdWithVCode(int LocatieId, string VCode);

public class AddressSyncService(IDocumentStore store, IGrarHttpClient grarHttpClient, ILogger<AddressSyncService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var session = store.LightweightSession();

        try
        {
            logger.LogInformation($"Adressen synchroniseren werd gestart.");

            var resultsByAddressId = await GetNachtelijkeSyncFromLocatieLookupDocument();

            logger.LogInformation(JsonConvert.SerializeObject(resultsByAddressId));
            logger.LogInformation($"Adressen synchroniseren werd voltooid.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Adressen synchroniseren kon niet voltooid worden. {ex.Message}");
        }
        finally
        {
            await session.DisposeAsync();
        }
    }

    private async Task<IReadOnlyCollection<NachtelijkeAdresSync>> GetNachtelijkeSyncFromLocatieLookupDocument()
    {
        await using var session = store.LightweightSession();

        var locatieLookupDocuments = await session.Query<LocatieLookupDocument>().ToListAsync();

        return locatieLookupDocuments.GroupBy(g => g.AdresId)
                                     .Select(s => new NachtelijkeAdresSync(s.Key,
                                                                                  s.Select(x => new LocatieIdWithVCode(
                                                                                               x.LocatieId, x.VCode)).ToList()))
                                     .ToArray();
    }
}
