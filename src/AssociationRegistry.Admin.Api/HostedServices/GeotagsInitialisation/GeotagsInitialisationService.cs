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

public class GeotagsInitialisationService
{
    private readonly IDocumentStore _store;
    private readonly IMartenOutbox _outbox;
    private readonly IVerenigingenWithoutGeotagsQuery _query;
    private readonly INutsLauFromGrarFetcher _nutsLauFromGrarFetcher;
    private readonly IPostcodesFromGrarFetcher _postcodesFromGrarFetcher;
    private readonly ILogger<GeotagsInitialisationService> _logger;

    public GeotagsInitialisationService(
        IDocumentStore store,
        IMartenOutbox outbox,
        IVerenigingenWithoutGeotagsQuery query,
        IPostcodesFromGrarFetcher postcodesFromGrarFetcher,
        INutsLauFromGrarFetcher nutsLauFromGrarFetcher,
        ILogger<GeotagsInitialisationService> logger)
    {
        _store = store;
        _outbox = outbox;
        _query = query;
        _nutsLauFromGrarFetcher = nutsLauFromGrarFetcher;
        _postcodesFromGrarFetcher = postcodesFromGrarFetcher;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var session = _store.LightweightSession();

        var geotagMigrationDone = await session.Query<GoetagMigration>()
                                                                     .AnyAsync(x => x.Id == new GoetagMigration().Id, cancellationToken);

        while (!geotagMigrationDone)
        {
            try
            {
                session.Store(new GoetagMigration());

                throw new Exception();
                await SyncNutsLauInfo(session, cancellationToken);

                await MigrateGeotags(cancellationToken, session);

                await session.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await Task.Delay(10.Seconds());
            }

            geotagMigrationDone = await session.Query<GoetagMigration>()
                                                   .AnyAsync(x => x.Id == new GoetagMigration().Id, cancellationToken);
        }
    }



    private async Task SyncNutsLauInfo(IDocumentSession session, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start syncing nuts lau info");

        var postcodes = await _postcodesFromGrarFetcher.FetchPostalCodes(cancellationToken);

        _logger.LogInformation($"PostcodesFromGrarFetcher returned {postcodes.Length} postcodes.");

        var nutsLauInfo = await _nutsLauFromGrarFetcher.GetFlemishAndBrusselsNutsAndLauByPostcode(postcodes, cancellationToken);

        _logger.LogInformation($"NutsLauFromGrarFetcher returned {nutsLauInfo.Length} nuts lau info.");

        if (nutsLauInfo.Length != 0)
        {
            session.DeleteWhere<PostalNutsLauInfo>(n => true);
            session.Store(nutsLauInfo);
        }
        else
        {
            throw new Exception("try syncing again");
        }

        _logger.LogInformation("Stopped syncing nuts lau info.");
    }

    private async Task MigrateGeotags(CancellationToken cancellationToken, IDocumentSession session)
    {
        _logger.LogInformation("GeotagsInitialisationService is starting.");
        _outbox.Enroll(session);

        var verenigingenWithoutGeotags = await _query.ExecuteAsync(cancellationToken);
        var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;

        foreach (var verenigingWithoutGeotag in verenigingenWithoutGeotags)
        {
            var command = new CommandEnvelope<InitialiseerGeotagsCommand>(
                new InitialiseerGeotagsCommand(VCode.Create(verenigingWithoutGeotag)), metadata);

            await _outbox.PublishAsync(command);
        }

        // for (int i = 0; i < 10; i++)
        // {
        //     _logger.LogInformation($"fv send {i}");
        //
        //     await Task.Delay(1.Seconds(), cancellationToken);
        //
        //     var command = new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
        //         new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand(VerenigingsNaam.Create("test-2"), null, null, null, Doelgroep.Create(10,50), false,[], [], [], [], []), metadata);
        //     //var command = new CommandEnvelope<InitialiseerGeotagsCommand>(new InitialiseerGeotagsCommand(VCode.Create(verenigingWithoutGeotag)), metadata);
        //     await _outbox.PublishAsync(command);
        // }


        _logger.LogInformation("GeotagsInitialisationService is stopping.");
    }
}

public record GoetagMigration()
{
    [Identity]
    public string Id => "migration2025-1";
}
