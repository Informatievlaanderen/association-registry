namespace AssociationRegistry.EventStore;

using Framework;
using Marten;
using Vereniging;
using Vereniging.Exceptions;

public class VerenigingsRepository : IVerenigingsRepository
{
    private readonly IEventStore _eventStore;

    public VerenigingsRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        var events = vereniging.UncommittedEvents.ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.Save(vereniging.VCode, metadata, cancellationToken, events);
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

        return await _eventStore.Save(vereniging.VCode, session, metadata, cancellationToken, events);
    }

    public async Task<TVereniging> Load<TVereniging>(VCode vCode, long? expectedVersion)
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(vCode, expectedVersion);
        ThrowIfVerwijderd(verenigingState);

        var vereniging = new TVereniging();
        vereniging.Hydrate(verenigingState);

        return vereniging;
    }

    public async Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, long? expectedVersion)
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(kboNummer, expectedVersion);
        var verenigingMetRechtspersoonlijkheid = new VerenigingMetRechtspersoonlijkheid();
        verenigingMetRechtspersoonlijkheid.Hydrate(verenigingState);

        return verenigingMetRechtspersoonlijkheid;
    }

    public async Task<bool> IsVerwijderd(VCode vCode)
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(vCode, null);

        return verenigingState.IsVerwijderd;
    }

    private void ThrowIfVerwijderd(VerenigingState verenigingState)
    {
        if (verenigingState.IsVerwijderd)
            throw new VerenigingWerdVerwijderd(verenigingState.VCode);
    }
}
