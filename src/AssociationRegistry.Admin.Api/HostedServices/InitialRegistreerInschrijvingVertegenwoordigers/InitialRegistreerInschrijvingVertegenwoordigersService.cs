namespace AssociationRegistry.Admin.Api.HostedServices.InitialRegistreerInschrijvingVertegenwoordigers;

using CommandHandling.DecentraalBeheer.Acties.Geotags.InitialiseerGeotags;
using CommandHandling.InschrijvingenVertegenwoordigers;
using DecentraalBeheer.Vereniging;
using Framework;
using Grar.NutsLau;
using JasperFx.Core;
using Magda.Persoon;
using Marten;
using Marten.Schema;
using Queries;
using Vereniging;
using Wolverine.Marten;
public class InitialRegistreerInschrijvingVertegenwoordigersService: BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDocumentStore _store;
    private readonly InitialiseerRegistreerInschrijvingOptions _options;
    private readonly ILogger<InitialRegistreerInschrijvingVertegenwoordigersService> _logger;
    public InitialRegistreerInschrijvingVertegenwoordigersService(
        IServiceScopeFactory scopeFactory,
        IDocumentStore store,
        InitialiseerRegistreerInschrijvingOptions options,
        ILogger<InitialRegistreerInschrijvingVertegenwoordigersService> logger)
    {
        _scopeFactory = scopeFactory;
        _store = store;
        _options = options;
        _logger = logger;

    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IMartenOutbox>();
        var query = scope.ServiceProvider.GetRequiredService<INietKboVerenigingenVCodesQuery>();

        var session = _store.LightweightSession();
        outbox.Enroll(session);
        var repository = new InitialRegistreerInschrijvingVertegenwoordigerRepository(session, _options);
        var migrationRanToCompletion = await repository.DidInitialisationAlreadyRunToCompletion(cancellationToken);
        while (!migrationRanToCompletion)
        {
            _logger.LogInformation("Start Initial RegistreerInschrijving Vertegenwoordigers");
            try
            {
                repository.AddInitialisationRecord();
                var vertegenwoordigers = await query.ExecuteAsync(cancellationToken);

                foreach (var vCode in vertegenwoordigers)
                {
                    await outbox.SendAsync(new CommandEnvelope<SchrijfVertegenwoordigersInMessage>(
                                               new SchrijfVertegenwoordigersInMessage(vCode.Value),
                                               CommandMetadata.ForDigitaalVlaanderenProcess));
                }

                await session.SaveChangesAsync(cancellationToken);

            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error while migrating geotags");
                await Task.Delay(10.Seconds(), cancellationToken);

            }
            migrationRanToCompletion = await repository.DidInitialisationAlreadyRunToCompletion(cancellationToken);

        }
        _logger.LogInformation("Initialisation of inschrijvingen completed successfully.");

    }
}

public class InitialiseerRegistreerInschrijvingOptions
{
    public string MigratieId { get; set; } = "initialisatie-202512";
}

public record InitialisatieInschrijvingenDocument()
{
    [Identity]
    public string Id { get; set; }
}

public class InitialRegistreerInschrijvingVertegenwoordigerRepository
{
    private readonly IDocumentSession _session;
    private readonly InitialiseerRegistreerInschrijvingOptions _options;

    public InitialRegistreerInschrijvingVertegenwoordigerRepository(IDocumentSession session, InitialiseerRegistreerInschrijvingOptions options)
    {
        _session = session;
        _options = options;
    }

    public async Task<bool> DidInitialisationAlreadyRunToCompletion(CancellationToken cancellationToken)
        => await _session.Query<InitialisatieInschrijvingenDocument>()
                         .AnyAsync(x => x.Id == _options.MigratieId, cancellationToken);

    public void AddInitialisationRecord()
    {
        _session.Insert(new InitialisatieInschrijvingenDocument() { Id = _options.MigratieId });
    }
}
