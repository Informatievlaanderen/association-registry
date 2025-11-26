namespace AssociationRegistry.Admin.ProjectionHost.Projections.Bewaartermijn;

using Events;
using Marten.Events.Aggregation;
using Schema.Bewaartermijn;

public class BewaartermijnProjection : SingleStreamProjection<BewaartermijnDocument, string>
{
    public BewaartermijnProjection()
    {
        CreateEvent<BewaartermijnWerdGestart>(
            @event => new BewaartermijnDocument(
                @event.BewaartermijnId,
                @event.VCode,
                @event.VertegenwoordigerId,
                @event.Vervaldag));
    }
}
