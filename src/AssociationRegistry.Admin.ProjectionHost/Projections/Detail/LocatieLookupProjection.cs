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
        Options.DeleteViewTypeOnTeardown<LocatieLookupDocument>();
    }

    public async Task Project(IEvent<AdresWerdOvergenomenUitAdressenregister> @event, IDocumentOperations ops)
        => await Upsert(@event, ops, LocatieLookupDocument.GetKey(@event.StreamKey, @event.Data.LocatieId), LocatieLookupProjector.Apply);

    public void Project(IEvent<AdresWerdNietGevondenInAdressenregister> @event, IDocumentOperations ops)
        => ops.DeleteWhere<LocatieLookupDocument>(doc => doc.VCode == @event.StreamKey && doc.LocatieId == @event.Data.LocatieId);

    public void Project(IEvent<AdresNietUniekInAdressenregister> @event, IDocumentOperations ops)
        => ops.DeleteWhere<LocatieLookupDocument>(doc => doc.VCode == @event.StreamKey && doc.LocatieId == @event.Data.LocatieId);

    public void Project(IEvent<LocatieWerdVerwijderd> @event, IDocumentOperations ops)
        => ops.DeleteWhere<LocatieLookupDocument>(doc => doc.VCode == @event.StreamKey && doc.LocatieId == @event.Data.Locatie.LocatieId);

    public void Project(IEvent<VerenigingWerdVerwijderd> @event, IDocumentOperations ops)
        => ops.DeleteWhere<LocatieLookupDocument>(doc => doc.VCode == @event.StreamKey);

    private static async Task Upsert<T>(
        IEvent<T> @event,
        IDocumentOperations ops,
        string key,
        Action<IEvent<T>, LocatieLookupDocument> action) where T : notnull
    {
        var doc = await ops.LoadAsync<LocatieLookupDocument>(key);

        if (doc is null)
        {
            doc = new LocatieLookupDocument()
            {
                Key = key,
                VCode = @event.StreamKey,
            };

            ops.Insert(doc);
        }

        action(@event, doc);
        LocatieLookupProjector.UpdateMetadata(@event, doc);

        ops.Store(doc);
    }
}
