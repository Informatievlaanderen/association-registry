namespace AssociationRegistry.Admin.AddressSync;

using Amazon.SQS;
using Infrastructure.ConfigurationBindings;
using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Schema.Detail;
using System.Text.Json;

public record NachtelijkeAdresSyncMessage(string AdresId, List<LocatieIdWithVCode> LocatieIdWithVCodes);
public record LocatieIdWithVCode(int LocatieId, string VCode);

public class AddressSyncService(IDocumentStore store, IAmazonSQS sqsClient, AddressSyncOptions options, ILogger<AddressSyncService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var session = store.LightweightSession();

        try
        {
            logger.LogInformation($"Adressen synchroniseren werd gestart.");

            var messages = await GetNachtelijkeSyncMessagesFromLocatieLookupDocument();

            foreach (var message in messages)
            {
                await sqsClient.SendMessageAsync(
                    options.QueueUrl,
                    JsonSerializer.Serialize(message));
            }

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

    private async Task<IReadOnlyCollection<NachtelijkeAdresSyncMessage>> GetNachtelijkeSyncMessagesFromLocatieLookupDocument()
    {
        using var session = store.LightweightSession();

        var locatieLookupDocuments = await session.Query<LocatieLookupDocument>()
                                                  .ToListAsync();

        return locatieLookupDocuments.GroupBy(g => g.AdresId)
                                     .Select(s => new NachtelijkeAdresSyncMessage(s.Key,
                                                                                  s.Select(x => new LocatieIdWithVCode(
                                                                                               x.LocatieId, x.VCode)).ToList()))
                                     .ToArray();
    }
}
