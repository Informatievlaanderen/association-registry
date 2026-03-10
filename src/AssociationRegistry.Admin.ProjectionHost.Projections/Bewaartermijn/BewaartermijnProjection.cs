namespace AssociationRegistry.Admin.ProjectionHost.Projections.Bewaartermijn;

using Events;
using Formats;
using Framework;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten.Events.Aggregation;
using Schema.Bewaartermijn;

public class BewaartermijnProjection : SingleStreamProjection<BewaartermijnDocument, string>
{
    public static readonly ShardName ShardName = new("beheer.postgres.bewaartermijn");

    public BewaartermijnProjection()
    {
        Name = ShardName.Name;
    }

    public BewaartermijnDocument Create(IEvent<BewaartermijnWerdGestartV2> @event)
    {
        var tijdstip = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip);

        return new BewaartermijnDocument(
            @event.Data.BewaartermijnId,
            @event.Data.VCode,
            @event.Data.BewaartermijnType,
            @event.Data.RecordId,
            @event.Data.Reden,
            BewaartermijnStatus.StatusGepland.Naam,
            @event.Data.Vervaldag,
            [new BewaartermijnGebeurtenis(BewaartermijnStatus.StatusGepland.Naam, tijdstip)]
        );
    }

    public BewaartermijnDocument Apply(BewaartermijnDocument doc, IEvent<BewaartermijnWerdVerlopen> @event)
    {
        var tijdstip = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip);

        return doc with
        {
            Status = BewaartermijnStatus.StatusVerlopen.Naam,
            Gebeurtenissen = doc
                .Gebeurtenissen.Append(new BewaartermijnGebeurtenis(BewaartermijnStatus.StatusVerlopen.Naam, tijdstip))
                .ToArray(),
        };
    }
}
