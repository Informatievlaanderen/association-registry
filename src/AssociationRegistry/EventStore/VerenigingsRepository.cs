namespace AssociationRegistry.EventStore;

using Events;
using Framework;
using Vereniging;

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
        var events = vereniging.UncommittedEvents.Select(Downcast).ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.Save(vereniging.VCode, metadata, cancellationToken, events);
    }

    private IEvent Downcast(IEvent @event)
        => @event switch
        {
            VertegenwoordigerWerdToegevoegd vertegenwoordigerWerdToegevoegd => new VertegenwoordigerWerdToegevoegdEncrypted(
                vertegenwoordigerWerdToegevoegd.VertegenwoordigerId, vertegenwoordigerWerdToegevoegd.Insz,
                vertegenwoordigerWerdToegevoegd.IsPrimair, vertegenwoordigerWerdToegevoegd.Roepnaam, vertegenwoordigerWerdToegevoegd.Rol,
                new string(vertegenwoordigerWerdToegevoegd.Voornaam.Reverse().ToArray()), vertegenwoordigerWerdToegevoegd.Achternaam,
                vertegenwoordigerWerdToegevoegd.Email,
                vertegenwoordigerWerdToegevoegd.Telefoon, vertegenwoordigerWerdToegevoegd.Mobiel,
                vertegenwoordigerWerdToegevoegd.SocialMedia),
            _ => @event,
        };

    public async Task<TVereniging> Load<TVereniging>(VCode vCode, long? expectedVersion)
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(vCode);

        var vereniging = new TVereniging();
        vereniging.Hydrate(verenigingState);

        if (expectedVersion is not null && verenigingState.Version != expectedVersion)
            throw new UnexpectedAggregateVersionException();

        return vereniging;
    }

    public async Task<VCodeAndNaam?> GetVCodeAndNaam(KboNummer kboNummer)
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(kboNummer);

        if (verenigingState == null)
            return null;

        return new VCodeAndNaam(verenigingState.VCode, verenigingState.Naam);
    }

    public record VCodeAndNaam(VCode? VCode, VerenigingsNaam VerenigingsNaam)
    {
        public static VCodeAndNaam Fallback(KboNummer kboNummer)
            => new(
                VCode: null,
                VerenigingsNaam.Create($"Moeder {kboNummer}"));
    }
}
