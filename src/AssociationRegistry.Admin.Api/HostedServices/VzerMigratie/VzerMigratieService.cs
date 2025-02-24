namespace AssociationRegistry.Admin.Api.HostedServices.VzerMigratie;

using Events;
using Marten;
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
}
