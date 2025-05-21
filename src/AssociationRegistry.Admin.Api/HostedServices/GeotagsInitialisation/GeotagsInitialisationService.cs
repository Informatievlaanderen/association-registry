namespace AssociationRegistry.Admin.Api.HostedServices.GeotagsInitialisation;

using DecentraalBeheer.Geotags.InitialiseerGeotags;
using Events;
using Framework;
using Humanizer;
using Marten;
using NodaTime;
using NodaTime.Text;
using Vereniging;
using Wolverine;

public class GeotagsInitialisationService : BackgroundService
{
    private readonly IDocumentStore _store;
    private readonly IMessageBus _bus;
    private readonly ILogger<GeotagsInitialisationService> _logger;

    public GeotagsInitialisationService(IDocumentStore store, IMessageBus bus, ILogger<GeotagsInitialisationService> logger)
    {
        _store = store;
        _bus = bus;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("GeotagsInitialisationService is starting.");

        var verenigingenWithoutGeotags = await GetVerenigingenWithoutGeotags(session, stoppingToken);

        _logger.LogInformation(
            "Found {Registrations} FeitelijkeVerenigingWerdGeregistreerd. Found {AlreadyMigrated} FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid. Verenigingen to migrate: {RegistrationsWithoutMigrations}",
            verenigingenWithoutGeotags.Count());

        await using var lockSession = _store.LightweightSession();

        var migrationEnded = await lockSession.Events.QueryRawEventDataOnly<MigrationEnded>().SingleOrDefaultAsync();
        var uniqueWorkerId = Guid.NewGuid();


        while (migrationEnded is null)
        {
            try
            {
                var migrationStarted = lockSession.Events.StartStream("migration2025", new MigrationStarted(uniqueWorkerId));
                await lockSession.SaveChangesAsync(stoppingToken);

                foreach (var verenigingWithoutGeotag in verenigingenWithoutGeotags)
                {
                    await using var session = _store.LightweightSession();

                    SetHeaders(new CommandMetadata(EventStore.EventStore.DigitaalVlaanderenOvoNumber, SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(),
                                                   null), session);

                    var command = new InitialiseerGeotagsCommand(verenigingWithoutGeotag);
                    await _bus.InvokeAsync(command, stoppingToken);

                    lockSession.Events.Append("migration2025", new MigrationProcessed(uniqueWorkerId));
                }

                lockSession.Events.Append("migration2025", new MigrationEnded());

            }
            catch
            {
                await Task.Delay(1.Minutes());
                migrationEnded = await lockSession.Events.QueryRawEventDataOnly<MigrationEnded>().SingleOrDefaultAsync();
            }
        }



        await StopAsync(stoppingToken);
    }

    private async Task<string[]> GetVerenigingenWithoutGeotags(IDocumentSession session, CancellationToken stoppingToken)
    {
        var vzers = await session.Events.QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>().Select(x => x.VCode)
                                 .ToListAsync();
        var kboVerenigingen = await session.Events.QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>().Select(x => x.VCode)
                                           .ToListAsync();
        var gemigreerdeVzers = await session.Events.QueryRawEventDataOnly<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid>().Select(x => x.VCode)
                                            .ToListAsync();

        var geotagsWerdenBepaald = await session.Events.QueryRawEventDataOnly<GeotagsWerdenBepaald>().Select(x => x.VCode)
                                                .ToListAsync();

        return vzers.
               Concat(kboVerenigingen)
              .Concat(gemigreerdeVzers)
              .Except(geotagsWerdenBepaald)
              .ToArray();
    }

    private static void SetHeaders(CommandMetadata metadata, IDocumentSession session)
    {
        session.SetHeader(MetadataHeaderNames.Initiator, metadata.Initiator);
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(metadata.Tijdstip));
        session.CorrelationId = metadata.CorrelationId.ToString();
    }
}

public record MigrationProcessed(Guid UniqueWorkerId);

public record MigrationStarted(Guid UniqueWorkerId)
{
}

public record MigrationEnded
{
}
