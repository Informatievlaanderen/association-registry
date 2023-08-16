namespace AssociationRegistry.EventStore;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Events;
using Framework;
using JasperFx.Core.Reflection;
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

    public async Task<StreamActionResult> Save(string aggregateId, CommandMetadata metadata, CancellationToken cancellationToken = default, params IEvent[] events)
    {
        await using var session = _documentStore.OpenSession();

        try
        {
            StreamAction streamAction = null!;
            foreach (var e in events)
            {
                streamAction = SaveEventToStream(aggregateId, metadata, e, session, metadata.ExpectedVersion is null ? -1 : metadata.ExpectedVersion.Value + 1);
            }

            await session.SaveChangesAsync(cancellationToken);
            return new StreamActionResult(streamAction.Events.Max(@event => @event.Sequence), streamAction.Version);
        }
        catch (EventStreamUnexpectedMaxEventIdException)
        {
            throw new UnexpectedAggregateVersionException();
        }
    }

    private static StreamAction SaveEventToStream(string aggregateId, CommandMetadata metadata, IEvent @event, IDocumentSession session, long nextVersion = -1)
    {
        SetHeaders(metadata, session, @event.GetType());

        if (nextVersion != -1)
            return session.Events.Append(aggregateId, nextVersion, @event);

        return session.Events.Append(aggregateId, @event);
    }

    private static void SetHeaders(CommandMetadata metadata, IDocumentSession session, Type eventType)
    {
        var initiator = metadata.Initiator;
        if (eventType == typeof(ContactgegevenWerdOvergenomenUitKBO) || eventType == typeof(MaatschappelijkeZetelWerdOvergenomenUitKbo))
            initiator = DigitaalVlaanderenOvoNumber;

        session.SetHeader(MetadataHeaderNames.Initiator, initiator);
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(metadata.Tijdstip));
    }

    public async Task<T> Load<T>(string id) where T : class, IHasVersion, new()
    {
        await using var session = _documentStore.OpenSession();

        return await session.Events.AggregateStreamAsync<T>(id) ??
               throw new AggregateNotFoundException(id, typeof(T));
    }

    public async Task<T?> Load<T>(KboNummer kboNummer) where T : class, IHasVersion, new()
    {
        await using var session = _documentStore.OpenSession();

        var id = (await session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
            .Where(geregistreerd => geregistreerd.KboNummer == kboNummer)
            .SingleOrDefaultAsync())?.VCode;

        if (string.IsNullOrEmpty(id))
            return null;

        return await Load<T>(id);
    }
}
