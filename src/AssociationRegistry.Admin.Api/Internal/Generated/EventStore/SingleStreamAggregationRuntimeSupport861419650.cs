// <auto-generated/>
#pragma warning disable
using Marten;
using Marten.Events.Aggregation;
using Marten.Internal.Storage;
using System;
using System.Linq;

namespace Marten.Generated.EventStore
{
    // START: SingleStreamAggregationLiveAggregation861419650
    public class SingleStreamAggregationLiveAggregation861419650 : Marten.Events.Aggregation.SyncLiveAggregatorBase<AssociationRegistry.Vereniging.Vereniging>
    {
        private readonly Marten.Events.Aggregation.SingleStreamAggregation<AssociationRegistry.Vereniging.Vereniging> _singleStreamAggregation;

        public SingleStreamAggregationLiveAggregation861419650(Marten.Events.Aggregation.SingleStreamAggregation<AssociationRegistry.Vereniging.Vereniging> singleStreamAggregation)
        {
            _singleStreamAggregation = singleStreamAggregation;
        }



        public override AssociationRegistry.Vereniging.Vereniging Build(System.Collections.Generic.IReadOnlyList<Marten.Events.IEvent> events, Marten.IQuerySession session, AssociationRegistry.Vereniging.Vereniging snapshot)
        {
            if (!events.Any()) return null;
            AssociationRegistry.Vereniging.Vereniging vereniging = null;
            snapshot ??= Create(events[0], session);
            foreach (var @event in events)
            {
                snapshot = Apply(@event, snapshot, session);
            }

            return snapshot;
        }


        public AssociationRegistry.Vereniging.Vereniging Create(Marten.Events.IEvent @event, Marten.IQuerySession session)
        {
            return new AssociationRegistry.Vereniging.Vereniging();
        }


        public AssociationRegistry.Vereniging.Vereniging Apply(Marten.Events.IEvent @event, AssociationRegistry.Vereniging.Vereniging aggregate, Marten.IQuerySession session)
        {
            switch (@event)
            {
                case Marten.Events.IEvent<object> event_Object47:
                    aggregate.Apply(event_Object47.Data);
                    break;
            }

            return aggregate;
        }

    }

    // END: SingleStreamAggregationLiveAggregation861419650
    
    
    // START: SingleStreamAggregationInlineHandler861419650
    public class SingleStreamAggregationInlineHandler861419650 : Marten.Events.Aggregation.AggregationRuntime<AssociationRegistry.Vereniging.Vereniging, string>
    {
        private readonly Marten.IDocumentStore _store;
        private readonly Marten.Events.Aggregation.IAggregateProjection _projection;
        private readonly Marten.Events.Aggregation.IEventSlicer<AssociationRegistry.Vereniging.Vereniging, string> _slicer;
        private readonly Marten.Internal.Storage.IDocumentStorage<AssociationRegistry.Vereniging.Vereniging, string> _storage;
        private readonly Marten.Events.Aggregation.SingleStreamAggregation<AssociationRegistry.Vereniging.Vereniging> _singleStreamAggregation;

        public SingleStreamAggregationInlineHandler861419650(Marten.IDocumentStore store, Marten.Events.Aggregation.IAggregateProjection projection, Marten.Events.Aggregation.IEventSlicer<AssociationRegistry.Vereniging.Vereniging, string> slicer, Marten.Internal.Storage.IDocumentStorage<AssociationRegistry.Vereniging.Vereniging, string> storage, Marten.Events.Aggregation.SingleStreamAggregation<AssociationRegistry.Vereniging.Vereniging> singleStreamAggregation) : base(store, projection, slicer, storage)
        {
            _store = store;
            _projection = projection;
            _slicer = slicer;
            _storage = storage;
            _singleStreamAggregation = singleStreamAggregation;
        }



        public override async System.Threading.Tasks.ValueTask<AssociationRegistry.Vereniging.Vereniging> ApplyEvent(Marten.IQuerySession session, Marten.Events.Projections.EventSlice<AssociationRegistry.Vereniging.Vereniging, string> slice, Marten.Events.IEvent evt, AssociationRegistry.Vereniging.Vereniging aggregate, System.Threading.CancellationToken cancellationToken)
        {
            switch (evt)
            {
                case Marten.Events.IEvent<object> event_Object48:
                    aggregate ??= new AssociationRegistry.Vereniging.Vereniging();
                    aggregate.Apply(event_Object48.Data);
                    return aggregate;
            }

            return aggregate;
        }


        public AssociationRegistry.Vereniging.Vereniging Create(Marten.Events.IEvent @event, Marten.IQuerySession session)
        {
            return new AssociationRegistry.Vereniging.Vereniging();
        }

    }

    // END: SingleStreamAggregationInlineHandler861419650
    
    
}

