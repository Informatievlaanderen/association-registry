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
        => await Upsert(@event, ops, LocatieLookupProjector.Apply);

    public async Task Project(IEvent<AdresWerdNietGevondenInAdressenregister> @event, IDocumentOperations ops)
        => await UpdateOrDeleteEntryOrDeleteDocument(@event.StreamKey, @event.Data.LocatieId, @event, ops);

    public async Task Project(IEvent<AdresNietUniekInAdressenregister> @event, IDocumentOperations ops)
        => await UpdateOrDeleteEntryOrDeleteDocument(@event.StreamKey, @event.Data.LocatieId, @event, ops);

    public async Task Project(IEvent<LocatieWerdVerwijderd> @event, IDocumentOperations ops)
        => await UpdateOrDeleteEntryOrDeleteDocument(@event.StreamKey, @event.Data.Locatie.LocatieId, @event, ops);

    public void Project(IEvent<VerenigingWerdVerwijderd> @event, IDocumentOperations ops)
        => ops.Delete<LocatieLookupDocument>(@event.StreamKey);

    private static async Task Upsert<T>(
        IEvent<T> @event,
        IDocumentOperations ops,
        Action<IEvent<T>, LocatieLookupDocument> action) where T : notnull
    {
        var doc = await ops.LoadAsync<LocatieLookupDocument>(@event.StreamKey);

        if (doc is null)
        {
            doc = new LocatieLookupDocument { VCode = @event.StreamKey };
            ops.Insert(doc);
        }

        action(@event, doc);
        LocatieLookupProjector.UpdateMetadata(@event, doc);

        ops.Store(doc);
    }

    private static async Task UpdateOrDeleteEntryOrDeleteDocument(string vCode, int locatieId, IEvent @event, IDocumentOperations ops)
    {
        var doc = await ops.LoadAsync<LocatieLookupDocument>(vCode);

        if (doc is not null)
        {
            if (doc.Locaties.Any(loc => loc.LocatieId != locatieId))
            {
                doc.Locaties = doc.Locaties.Where(w => w.LocatieId != locatieId).ToArray();
                LocatieLookupProjector.UpdateMetadata(@event, doc);
                ops.Store(doc);

                return;
            }

            ops.Delete<LocatieLookupDocument>(vCode);
        }
    }
}
