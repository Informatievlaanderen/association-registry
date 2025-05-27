namespace AssociationRegistry.Admin.Api.HostedServices.GeotagsInitialisation;

using DecentraalBeheer.Geotags.InitialiseerGeotags;
using DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using Framework;
using Grar.Clients;
using Grar.NutsLau;
using Humanizer;
using Marten;
using Marten.Schema;
using Queries;
using Vereniging;
using Wolverine.Marten;

public class GeotagsInitialisationService: BackgroundService
{
    private readonly IDocumentStore _store;
    private readonly IMartenOutbox _outbox;
    private readonly IVerenigingenWithoutGeotagsQuery _query;
    private readonly INutsLauFromGrarFetcher _nutsLauFromGrarFetcher;
    private readonly ILogger<GeotagsInitialisationService> _logger;

    public GeotagsInitialisationService(
        IDocumentStore store,
        IMartenOutbox outbox,
        IVerenigingenWithoutGeotagsQuery query,
        INutsLauFromGrarFetcher nutsLauFromGrarFetcher,
        ILogger<GeotagsInitialisationService> logger)
    {
        _store = store;
        _outbox = outbox;
        _query = query;
        _nutsLauFromGrarFetcher = nutsLauFromGrarFetcher;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var session = _store.LightweightSession();

        var geotagMigrationDone = await session.Query<GeotagMigration>()
                                                                     .AnyAsync(x => x.Id == new GeotagMigration().Id, cancellationToken);

        while (!geotagMigrationDone)
        {
            try
            {
                session.Insert(new GeotagMigration());

                await SyncNutsLauInfo(session, cancellationToken);

                await MigrateGeotags(session, cancellationToken);

                await session.SaveChangesAsync(cancellationToken);
                _logger.LogWarning("Migrated geotags to {0}", session.Query<GeotagMigration>().Count());
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error while migrating geotags");

                await Task.Delay(10.Seconds());
            }

            geotagMigrationDone = await session.Query<GeotagMigration>()
                                                   .AnyAsync(x => x.Id == new GeotagMigration().Id, cancellationToken);
        }
    }



    private async Task SyncNutsLauInfo(IDocumentSession session, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start syncing nuts lau info");


        var nutsLauInfo = await _nutsLauFromGrarFetcher.GetFlemishAndBrusselsNutsAndLauByPostcode(cancellationToken);

        _logger.LogInformation($"NutsLauFromGrarFetcher returned {nutsLauInfo.Length} nuts lau infos.");

        if (nutsLauInfo.Length != 0)
        {
            session.DeleteWhere<PostalNutsLauInfo>(n => true);
            session.Store(nutsLauInfo);
            _logger.LogInformation("Stopped syncing nuts lau info.");
        }
        else
        {
            throw new Exception("No nuts lau info returned. Aborting migration. Retry will happen in a short while.");
        }
    }

    private async Task MigrateGeotags(IDocumentSession session, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting to prepare geotags migration outbox.");
        _outbox.Enroll(session);

        var verenigingenWithoutGeotags = await _query.ExecuteAsync(cancellationToken);
        var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;

        foreach (var verenigingWithoutGeotag in verenigingenWithoutGeotags)
        {
            var command = new CommandEnvelope<InitialiseerGeotagsCommand>(
                new InitialiseerGeotagsCommand(VCode.Create(verenigingWithoutGeotag)), metadata);

            await _outbox.SendAsync(command);
        }

        _logger.LogInformation("Geotag migrations sent to outbox, awaiting transaction completion.");
    }
}

public record GeotagMigration()
{
    [Identity]
    public string Id => "geotag-migration-202505";
}
