namespace AssociationRegistry.Admin.ProjectionHost.Projections.Bewaartermijn;

using Events;
using JasperFx.Events.Projections;
using Marten.Events.Aggregation;
using Schema.Bewaartermijn;

public class BewaartermijnProjection : SingleStreamProjection<BewaartermijnDocument, string>
{
    public static readonly ShardName ShardName = new("beheer.postgres.bewaartermijn");

    public BewaartermijnProjection()
    {
        Name = ShardName.Name;

        CreateEvent<BewaartermijnWerdGestart>(@event => new BewaartermijnDocument(
            @event.BewaartermijnId,
            @event.VCode,
            @event.VertegenwoordigerId,
            @event.Vervaldag
        ));
    }
}
