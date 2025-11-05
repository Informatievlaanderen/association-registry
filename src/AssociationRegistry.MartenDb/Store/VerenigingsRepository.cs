namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using DecentraalBeheer.Vereniging;
using Marten;
using OpenTelemetry.Metrics;
using Persoonsgegevens;

public class VerenigingsRepository : IVerenigingsRepository
{
    private readonly IEventStore _eventStore;
    private readonly IVertegenwoordigerPersoonsgegevensService _vertegenwoordigerPersoonsgegevensService;

    public VerenigingsRepository(IEventStore eventStore, IVertegenwoordigerPersoonsgegevensService vertegenwoordigerPersoonsgegevensService)
    {
        _eventStore = eventStore;
        _vertegenwoordigerPersoonsgegevensService = vertegenwoordigerPersoonsgegevensService;
    }

    public async Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        var events = vereniging.UncommittedEvents.ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.Save(vereniging.VCode, vereniging.Version, metadata, cancellationToken, events);
    }

    public async Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
    {
        var events = vereniging.UncommittedEvents.ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.Save(vereniging.VCode, vereniging.Version, session, metadata, cancellationToken, events);
    }

    public async Task<StreamActionResult> SaveNew(VerenigingsBase vereniging, IDocumentSession session, CommandMetadata metadata, CancellationToken cancellationToken)
    {
        var events = vereniging.UncommittedEvents.ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.SaveNew(vereniging.VCode, session, metadata, cancellationToken, events);
    }

    public async Task<TVereniging> Load<TVereniging>(VCode vCode, CommandMetadata metadata, bool allowVerwijderdeVereniging = false, bool allowDubbeleVereniging = false)
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        RepositoryMetrics.RecordAggregateLoaded(metadata.ExpectedVersion.HasValue, metadata.Initiator);

        var verenigingState = await _eventStore.Load(vCode, metadata.ExpectedVersion,
            () => new VerenigingState()
            {
                VertegenwoordigerPersoonsgegevensService = _vertegenwoordigerPersoonsgegevensService,
            });

        if (!allowVerwijderdeVereniging)
            verenigingState.ThrowIfVerwijderd();

        if (!allowDubbeleVereniging)
            verenigingState.ThrowIfDubbel();

        var vereniging = new TVereniging();
        vereniging.Hydrate(verenigingState);

        return vereniging;
    }

    public async Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, CommandMetadata metadata)
    {
        RepositoryMetrics.RecordAggregateLoaded(metadata.ExpectedVersion.HasValue, metadata.Initiator);

        var vCode = await _eventStore.GetVCodeForKbo(kboNummer);
        if (vCode is null)
            throw new AggregateNotFoundException(kboNummer, typeof(VerenigingMetRechtspersoonlijkheid));

        var verenigingState = await _eventStore.Load(vCode, metadata.ExpectedVersion,
            () => new VerenigingState()
            {
                VertegenwoordigerPersoonsgegevensService = _vertegenwoordigerPersoonsgegevensService,
            });
        var verenigingMetRechtspersoonlijkheid = new VerenigingMetRechtspersoonlijkheid();
        verenigingMetRechtspersoonlijkheid.Hydrate(verenigingState);

        return verenigingMetRechtspersoonlijkheid;
    }

    public async Task<bool> IsVerwijderd(VCode vCode)
    {
        var verenigingState = await _eventStore.Load(vCode, null,
            () => new VerenigingState()
            {
                VertegenwoordigerPersoonsgegevensService = _vertegenwoordigerPersoonsgegevensService,
            });

        return verenigingState.IsVerwijderd;
    }

    public async Task<bool> IsDubbel(VCode vCode)
    {
        var verenigingState = await _eventStore.Load(vCode, null,
                                                     () => new VerenigingState()
                                                     {
                                                         VertegenwoordigerPersoonsgegevensService = _vertegenwoordigerPersoonsgegevensService,
                                                     });

        return verenigingState.VerenigingStatus is VerenigingStatus.StatusDubbel;
    }

    public async Task<bool> Exists(VCode vCode)
        => await _eventStore.Exists(vCode);

    public async Task<bool> Exists(KboNummer kboNummer)
        => await _eventStore.Exists(kboNummer);

}
