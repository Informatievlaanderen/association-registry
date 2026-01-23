namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.EventStore;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using DecentraalBeheer.Vereniging;
using Events;
using Marten;
using Vereniging;

public class VerenigingStateQueryService : IVerenigingStateQueryService
{
    private readonly IDocumentSession _session;

    public VerenigingStateQueryService(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<bool> IsVerwijderd(VCode vCode)
    {
        var verenigingState =
            await _session.Events.AggregateStreamAsync<VerenigingState>(vCode)
            ?? throw new AggregateNotFoundException(vCode, typeof(VerenigingState));

        return verenigingState.IsVerwijderd;
    }

    public async Task<bool> IsDubbel(VCode vCode)
    {
        var verenigingState =
            await _session.Events.AggregateStreamAsync<VerenigingState>(vCode)
            ?? throw new AggregateNotFoundException(vCode, typeof(VerenigingState));

        return verenigingState.VerenigingStatus is VerenigingStatus.StatusDubbel;
    }

    public async Task<bool> Exists(VCode vCode)
    {
        var streamState = await _session.Events.FetchStreamStateAsync(vCode);

        return streamState != null;
    }

    public async Task<bool> Exists(KboNummer kboNummer)
    {
        var streamState = await _session.Events.FetchStreamStateAsync(kboNummer);

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = await _session
            .Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
            .Where(x => x.KboNummer == kboNummer.Value)
            .SingleOrDefaultAsync();

        return streamState != null || verenigingMetRechtspersoonlijkheidWerdGeregistreerd != null;
    }
}
