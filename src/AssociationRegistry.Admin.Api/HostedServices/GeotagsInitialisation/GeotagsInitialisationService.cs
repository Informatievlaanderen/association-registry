namespace AssociationRegistry.Admin.Api.HostedServices.GeotagsInitialisation;

using DecentraalBeheer.Geotags.InitialiseerGeotags;
using Framework;
using Grar.NutsLau;
using JasperFx.Core;
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

                await MigrateGeotags(cancellationToken);

                await session.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error while migrating geotags");

                await Task.Delay(10.Seconds());
            }

            migrationRanToCompletion = await geotagMigrationRepository.DidMigrationAlreadyRunToCompletion(cancellationToken);
        }
        _logger.LogInformation("Initialisation of geotags completed successfully.");
    }

    private void ReplacePostalNutsLauInDb(PostalNutsLauInfo[] nutsLauInfo, IDocumentSession session)
    {
        _logger.LogInformation($"NutsLauFromGrarFetcher returned {nutsLauInfo.Length} nuts lau infos.");
        session.DeleteWhere<PostalNutsLauInfo>(_ => true);
        session.Store(nutsLauInfo);
    }

    private async Task MigrateGeotags(CancellationToken cancellationToken)
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

        _logger.LogInformation("{VerenigingenWithoutGeotags} Geotag initialisation commands sent to outbox, awaiting transaction completion.");
    }
}

public record GeotagMigration()
{
    private const string GeotagMigration202505 = "geotag-migration-202505";
    private const string GeotagMigration202506 = "geotag-migration-202506";

    [Identity]
    public string Id => GeotagMigration202506;
}
