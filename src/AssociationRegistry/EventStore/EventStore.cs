namespace AssociationRegistry.EventStore;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Events;
using Framework;
using Marten;
using Marten.Events;
using Marten.Exceptions;
using NodaTime.Text;
using Vereniging;
using IEvent = Framework.IEvent;

public class EventStore : IEventStore
{
    public const string DigitaalVlaanderenOvoNumber = "OVO002949";
    private readonly IDocumentStore _documentStore;

    public EventStore(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<StreamActionResult> Save(
        string aggregateId,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default,
        params IEvent[] events)
    {
        await using var session = _documentStore.LightweightSession();

        return await Save(aggregateId, session, metadata , cancellationToken, events);
    }

    public async Task<StreamActionResult> Save(
        string aggregateId,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events)
    {
        try
        {
            SetHeaders(metadata, session);

            TryLockForKboNumber(aggregateId, session, events.FirstOrDefault());

            var streamAction = AppendEvents(session, aggregateId, events, metadata.ExpectedVersion);

            await session.SaveChangesAsync(cancellationToken);

            return new StreamActionResult(streamAction.Events.Max(@event => @event.Sequence), streamAction.Version);
        }
        catch (EventStreamUnexpectedMaxEventIdException)
        {
            throw new UnexpectedAggregateVersionException();
        }
    }

private static void TryLockForKboNumber(string vCode, IDocumentSession session, IEvent? registreerEvent)
    {
        if (registreerEvent is VerenigingMetRechtspersoonlijkheidWerdGeregistreerd evnt)
            session.Events.StartStream<KboNummer>(evnt.KboNummer, new { VCode = vCode });
    }

    private static StreamAction AppendEvents(
        IDocumentSession session,
        string aggregateId,
        IReadOnlyCollection<IEvent> events,
        long? expectedVersion)
    {
        if (expectedVersion is not null)
            return session.Events.Append(aggregateId, expectedVersion.Value + events.Count, events);

        return session.Events.Append(aggregateId, events);
    }

    private static void SetHeaders(CommandMetadata metadata, IDocumentSession session)
    {
        session.SetHeader(MetadataHeaderNames.Initiator, metadata.Initiator);
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(metadata.Tijdstip));
        session.CorrelationId = metadata.CorrelationId.ToString();
    }

    private static bool IsEventFromDigitaalVlaanderen(Type eventType)
        => new[]
        {
            typeof(ContactgegevenWerdOvergenomenUitKBO),
            typeof(MaatschappelijkeZetelWerdOvergenomenUitKbo),
            typeof(ContactgegevenKonNietOvergenomenWordenUitKBO),
            typeof(MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo),
        }.Contains(eventType);

    public async Task<T> Load<T>(string id) where T : class, IHasVersion, new()
    {
        await using var session = _documentStore.LightweightSession();

        return await session.Events.AggregateStreamAsync<T>(id) ??
               throw new AggregateNotFoundException(id, typeof(T));
    }

    public async Task<T?> Load<T>(KboNummer kboNummer) where T : class, IHasVersion, new()
    {
        await using var session = _documentStore.LightweightSession();

        var id = (await session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                               .Where(geregistreerd => geregistreerd.KboNummer == kboNummer)
                               .SingleOrDefaultAsync())?.VCode;

        if (string.IsNullOrEmpty(id))
            throw new AggregateNotFoundException(kboNummer, typeof(T));

        return await Load<T>(id);
    }
}
