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
using Repositories;
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
        _outbox.Enroll(session);
        var geotagMigrationRepository = new GeotagMigrationRepository(session);

        var migrationRanToCompletion = await geotagMigrationRepository.DidMigrationAlreadyRunToCompletion(cancellationToken);

        while (!migrationRanToCompletion)
        {
            try
            {
                geotagMigrationRepository.AddMigrationRecord();

                _logger.LogInformation("Start syncing nuts lau info");

                var nutsLauInfo = await _nutsLauFromGrarFetcher.GetFlemishAndBrusselsNutsAndLauByPostcode(cancellationToken);

                if (nutsLauInfo.Length == 0)
                    throw new Exception("No nuts lau info returned. Aborting migration. Retry will happen in a short while.");

                ReplacePostalNutsLauInDb(nutsLauInfo, session);

                await MigrateGeotags(session, cancellationToken);

                await session.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error while migrating geotags");

                await Task.Delay(10.Seconds());
            }

            migrationRanToCompletion = await geotagMigrationRepository.DidMigrationAlreadyRunToCompletion(cancellationToken);
        }
    }

    private void ReplacePostalNutsLauInDb(PostalNutsLauInfo[] nutsLauInfo, IDocumentSession session)
    {
        _logger.LogInformation($"NutsLauFromGrarFetcher returned {nutsLauInfo.Length} nuts lau infos.");
        session.DeleteWhere<PostalNutsLauInfo>(_ => true);
        session.Store(nutsLauInfo);
    }

    private async Task MigrateGeotags(IDocumentSession session, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting to prepare geotags migration outbox.");

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
