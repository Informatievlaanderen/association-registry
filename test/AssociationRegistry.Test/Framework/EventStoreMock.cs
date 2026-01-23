namespace AssociationRegistry.Test.Framework;

using AssociationRegistry.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using MartenDb.Store;
using Persoonsgegevens;
using Vereniging;

public class EventStoreMock : IEventStore
{
    public record SaveInvocation(string AggregateId, IEvent[] Events);

    public record LoadInvocation(string AggregateId, Type Type);

    private readonly IEvent[] _events;

    public EventStoreMock(params IEvent[] events)
    {
        _events = events;
    }

    public readonly List<SaveInvocation> SaveInvocations = new();

    public async Task<StreamActionResult> Save(
        string aggregateId,
        long aggregateVersion,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events
    ) =>
        await Save(
            aggregateId: aggregateId,
            aggregateVersion: aggregateVersion,
            metadata: metadata,
            cancellationToken: cancellationToken,
            events: events
        );

    public Task<T> Load<T>(string aggregateId, long? expectedVersion)
        where T : class, IHasVersion, new()
    {
        var result = new T();

        for (var i = 0; i < _events.Length; i++)
        {
            result = ((dynamic)result).Apply((dynamic)_events[i]);
            result.Version = i + 1;
        }

        return Task.FromResult(result);
    }

    public Task<T?> Load<T>(KboNummer kboNummer, long? expectedVersion)
        where T : class, IHasVersion, new() => Task.FromException<T?>(new NotImplementedException());

    public async Task<bool> Exists(VCode vCode) => true;

    public async Task<bool> Exists(KboNummer kboNummer) => throw new NotImplementedException();

    public async Task<StreamActionResult> SaveNew(
        string aggregateId,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events
    ) => throw new NotImplementedException();

    public async Task<VCode?> GetVCodeForKbo(string kboNummer) => throw new NotImplementedException();

    public async Task<StreamActionResult> SaveNewKbo(
        string aggregateId,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events
    ) => throw new NotImplementedException();

    public async Task<IReadOnlyList<VCode>> FilterVzerOnly(
        IEnumerable<VCode> vertegenwoordigerPersoonsgegevensByVCode
    ) => throw new NotImplementedException();
}
