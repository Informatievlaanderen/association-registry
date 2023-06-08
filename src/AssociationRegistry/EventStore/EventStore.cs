namespace AssociationRegistry.EventStore;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Events;
using Framework;
using JasperFx.Core.Reflection;
using Marten;
using Marten.Exceptions;
using NodaTime.Text;
using Vereniging;

public class EventStore : IEventStore
{
    private readonly IDocumentStore _documentStore;

    public EventStore(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<StreamActionResult> Save(string aggregateId, CommandMetadata metadata, CancellationToken cancellationToken = default, params IEvent[] events)
    {
        await using var session = _documentStore.OpenSession();

        session.SetHeader(MetadataHeaderNames.Initiator, metadata.Initiator);
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(metadata.Tijdstip));
        try
        {
            var streamAction = metadata.ExpectedVersion.HasValue ? session.Events.Append(aggregateId, metadata.ExpectedVersion.Value + events.Length, events.As<object[]>()) : session.Events.Append(aggregateId, events.As<object[]>());

            await session.SaveChangesAsync(cancellationToken);
            return new StreamActionResult(streamAction.Events.Max(@event => @event.Sequence), streamAction.Version);
        }
        catch (EventStreamUnexpectedMaxEventIdException)
        {
            throw new UnexpectedAggregateVersionException();
        }
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
