namespace AssociationRegistry.EventStore;

using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Framework;
using JasperFx.Core.Reflection;
using Marten;
using Marten.Exceptions;
using NodaTime.Text;
using IEvent = Framework.IEvent;

public class EventStore : IEventStore
{
    private readonly IDocumentStore _documentStore;

    public EventStore(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<StreamActionResult> Save(string aggregateId, CommandMetadata metadata, params IEvent[] events)
    {
        await using var session = _documentStore.OpenSession();

        session.SetHeader(MetadataHeaderNames.Initiator, metadata.Initiator);
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(metadata.Tijdstip));
        try
        {
            var streamAction = metadata.ExpectedVersion.HasValue ? session.Events.Append(aggregateId, metadata.ExpectedVersion.Value + events.Length, events.As<object[]>()) : session.Events.Append(aggregateId, events.As<object[]>());

            await session.SaveChangesAsync();
            return new StreamActionResult(streamAction.Events.Max(@event => @event.Sequence), streamAction.Version);
        }
        catch (EventStreamUnexpectedMaxEventIdException)
        {
            throw new UnexpectedAggregateVersionException();
        }
    }

    public async Task<T> Load<T>(string id) where T : class, IHasVersion
    {
        await using var session = _documentStore.OpenSession();

        var aggregate = await session.Events.AggregateStreamAsync<T>(id) ??
                        throw new AggregateNotFoundException(id, typeof(T));
        return aggregate;
    }
}
