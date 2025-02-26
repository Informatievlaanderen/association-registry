namespace AssociationRegistry.Admin.Api.HostedServices.VzerMigratie;

using Events;
using Framework;
using Marten;
using NodaTime;
using NodaTime.Text;
using Vereniging;

public class VzerMigratieService : BackgroundService
{
    private readonly IDocumentStore _store;
    private readonly ILogger<VzerMigratieService> _logger;

    public VzerMigratieService(IDocumentStore store, ILogger<VzerMigratieService> logger)
    {
        _store = store;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("VzerMigratieService is starting.");
        await using var session = _store.LightweightSession();

        var registraties = session.Events.QueryRawEventDataOnly<FeitelijkeVerenigingWerdGeregistreerd>();
        var migraties = session.Events.QueryRawEventDataOnly<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid>();

        var registratiesZonderMigraties =
            registraties
               .Select(x => x.VCode)
               .Where(x => !migraties.Select(x => x.VCode).Contains(x))
               .ToList();

        _logger.LogInformation(
            "Found {Registrations} FeitelijkeVerenigingWerdGeregistreerd. Found {AlreadyMigrated} FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid. Verenigingen to migrate: {RegistrationsWithoutMigrations}",
            registraties.Count(), migraties.Count(), registratiesZonderMigraties.Count());
        var succeededMigrations = 0;
        var failedMigrations = 0;

        SetHeaders(new CommandMetadata(EventStore.EventStore.DigitaalVlaanderenOvoNumber, SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(),
                                       null), session);

        foreach (var registratieZonderMigratie in registratiesZonderMigraties)
        {
            try
            {
                var stream = await session.Events.FetchForExclusiveWriting<VerenigingState>(registratieZonderMigratie, stoppingToken);

                if (stream.Aggregate.Verenigingstype != Verenigingstype.FeitelijkeVereniging)
                    continue;

                stream.AppendOne(new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(registratieZonderMigratie));

                await session.SaveChangesAsync(stoppingToken);
                succeededMigrations++;
            }
            catch (OperationCanceledException e)
            {
                failedMigrations++;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                failedMigrations++;
            }
        }

        await StopAsync(stoppingToken);
    }

    private static void SetHeaders(CommandMetadata metadata, IDocumentSession session)
    {
        session.SetHeader(MetadataHeaderNames.Initiator, metadata.Initiator);
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(metadata.Tijdstip));
        session.CorrelationId = metadata.CorrelationId.ToString();
    }
}
