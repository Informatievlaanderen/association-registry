namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Events;
using Marten;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Schema.Detail;

public class LocatieLookupProjection : MultiStreamProjection<LocatieLookupDocument, string>
{
    public LocatieLookupProjection()
    {
        // Needs a batch size of 1, because otherwise if Registered and NameChanged arrive in 1 batch/slice,
        // the newly persisted document from xxxWerdGeregistreerd is not in the
        // Query yet when we handle NaamWerdGewijzigd.
        // see also https://martendb.io/events/projections/event-projections.html#reusing-documents-in-the-same-batch
        Options.BatchSize = 1;
        Options.DeleteViewTypeOnTeardown<LocatieLookupDocument>();

        Identity<AdresWerdOvergenomenUitAdressenregister>(x => $"{x.VCode}-{x.LocatieId}");
        // Identity<LocatieWerdVerwijderd>(x =>
        // {
        //
        //     return $"{x.StreamKey}-{x.Data.Locatie.LocatieId}";
        // });

        CreateEvent<AdresWerdOvergenomenUitAdressenregister>(x =>
        {
            return new LocatieLookupDocument
            {
                AdresId = x.OvergenomenAdresUitAdressenregister.AdresId.Bronwaarde.Split('/').Last(),
                LocatieId = x.LocatieId,
                VCode = x.VCode
            };
        });


    }

    public void Apply(LocatieWerdVerwijderd @event, LocatieLookupDocument doc)
    {

    }

}
