﻿namespace AssociationRegistry.Public.ProjectionHost.Projections.Sequence;

using Events;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Marten.Internal.Sessions;
using Schema.Sequence;

public class PubliekVerenigingSequenceProjection : CustomProjection<PubliekVerenigingSequenceDocument, string>
{
    public PubliekVerenigingSequenceProjection()
    {
        AggregateByStream();

        var eventTypes = typeof(AssociationRegistry.Framework.IEvent).Assembly
                                                                     .GetTypes()
                                                                     .Where(t => typeof(AssociationRegistry.Framework.IEvent)
                                                                               .IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                                                                     .ToList();

        foreach (var eventType in eventTypes)
            IncludeType(eventType);
    }

    public override ValueTask ApplyChangesAsync(
        DocumentSessionBase session,
        EventSlice<PubliekVerenigingSequenceDocument, string> slice,
        CancellationToken cancellationToken,
        ProjectionLifecycle lifecycle = ProjectionLifecycle.Inline)
    {
        var aggregate = slice.Aggregate;

        foreach (var @event in slice.Events())
        {
            switch (@event)
            {
                case Event<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>:
                case Event<FeitelijkeVerenigingWerdGeregistreerd>:
                    aggregate = new PubliekVerenigingSequenceDocument { VCode = @event.StreamKey, Sequence = @event.Sequence };

                    break;

                default:
                    aggregate.Sequence = @event.Sequence;

                    break;
            }
        }

        if (aggregate is not null)
            session.Store(aggregate);

        return new ValueTask();
    }
}
