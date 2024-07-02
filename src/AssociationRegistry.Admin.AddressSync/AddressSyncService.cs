﻿namespace AssociationRegistry.Admin.AddressSync;

using EventStore;
using Framework;
using Grar.AddressSync;
using Grar.Models;
using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using Schema.Detail;
using System.Diagnostics.Contracts;

public record NachtelijkeAdresSyncVolgensAdresId(string AdresId, List<LocatieIdWithVCode> LocatieIdWithVCodes);
public record NachtelijkeAdresSyncVolgensVCode(string VCode, List<LocatieWithAdres> LocatieWithAdres);
public record LocatieIdWithVCode(int LocatieId, string VCode);

public class AddressSyncService(
    IDocumentStore store,
    TeSynchroniserenLocatieAdresMessageHandler handler,
    ITeSynchroniserenLocatiesFetcher teSynchroniserenLocatiesFetcher,
    ILogger<AddressSyncService> logger)
    : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var session = store.LightweightSession();

        try
        {
            logger.LogInformation($"Adressen synchroniseren werd gestart.");

            var messages = await teSynchroniserenLocatiesFetcher.GetTeSynchroniserenLocaties(session, cancellationToken);

            foreach (var synchroniseerLocatieMessage in messages)
            {
                await handler.Handle(synchroniseerLocatieMessage, cancellationToken);
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
}