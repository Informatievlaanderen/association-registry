namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Events;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Schema.Detail;

public class LocatieLookupProjection : EventProjection
{
    public LocatieLookupProjection()
    {
        // Needs a batch size of 1, because otherwise if Registered and NameChanged arrive in 1 batch/slice,
        // the newly persisted document from xxxWerdGeregistreerd is not in the
        // Query yet when we handle NaamWerdGewijzigd.
        // see also https://martendb.io/events/projections/event-projections.html#reusing-documents-in-the-same-batch
        Options.BatchSize = 1;
        Options.DeleteViewTypeOnTeardown<BeheerVerenigingDetailDocument>();
    }

    public async Task Project(IEvent<AdresWerdOvergenomenUitAdressenregister> @event, IDocumentOperations ops)
        => await Upsert(@event, ops, LocatieLookupProjector.Apply);

    // public async Task Project(IEvent<AdresWerdNietGevondenInAdressenregister> @event, IDocumentOperations ops)
    //     => ops.Delete<LocatieLookupDocument>(@event.StreamKey);
    //
    // public async Task Project(IEvent<AdresNietUniekInAdressenregister> @event, IDocumentOperations ops)
    //     => ops.Delete<LocatieLookupDocument>(@event.StreamKey);
    //
    // public async Task Project(IEvent<LocatieWerdVerwijderd> @event, IDocumentOperations ops)
    //     => ops.Delete<LocatieLookupDocument>(@event.StreamKey);
    //
    // public async Task Project(IEvent<VerenigingWerdVerwijderd> @event, IDocumentOperations ops)
    //     => ops.Delete<LocatieLookupDocument>(@event.StreamKey);

    private static async Task Upsert<T>(
        IEvent<T> @event,
        IDocumentOperations ops,
        Action<IEvent<T>, LocatieLookupDocument> action) where T : notnull
    {
        var doc = (await ops.LoadAsync<LocatieLookupDocument>(@event.StreamKey))!;

        if (doc is null)
        {
            doc = new LocatieLookupDocument()
            {
                VCode = @event.StreamKey
            };

            ops.Insert(doc);
        }

        action(@event, doc);
        LocatieLookupProjector.UpdateMetadata(@event, doc);

        ops.Store(doc);
    }
}
