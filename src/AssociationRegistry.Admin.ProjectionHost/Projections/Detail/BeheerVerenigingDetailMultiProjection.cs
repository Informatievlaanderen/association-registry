namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Events;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Schema.Detail;

public class BeheerVerenigingDetailMultiProjection : MultiStreamProjection<BeheerVerenigingDetailMultiDocument, string>
{
    public BeheerVerenigingDetailMultiProjection()
    {
        // Needs a batch size of 1, because otherwise if Registered and NameChanged arrive in 1 batch/slice,
        // the newly persisted document from xxxWerdGeregistreerd is not in the
        // Query yet when we handle NaamWerdGewijzigd.
        // see also https://martendb.io/events/projections/event-projections.html#reusing-documents-in-the-same-batch


        Identity<FeitelijkeVerenigingWerdGeregistreerd>(x => x.VCode);
        CreateEvent<IEvent<FeitelijkeVerenigingWerdGeregistreerd>>(BeheerVerenigingDetailMultiProjector.Create);

        Identity<IEvent<LocatieWerdToegevoegd>>(x => x.StreamKey);
        ProjectEvent<IEvent<LocatieWerdToegevoegd>>((doc,e) =>  BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Options.EnableDocumentTrackingByIdentity = true;
        Options.DeleteViewTypeOnTeardown<BeheerVerenigingDetailMultiDocument>();
    }
}
