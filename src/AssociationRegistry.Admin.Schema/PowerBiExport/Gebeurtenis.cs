namespace AssociationRegistry.Admin.Schema.PowerBiExport;

using Framework;
using IEvent = JasperFx.Events.IEvent;

public record Gebeurtenis(string Datum, string EventType, string Initiator, long Sequence)
{
    public static Gebeurtenis FromEvent(IEvent @event)
    {
        var instant = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip);

        return new Gebeurtenis(
            instant.ToString(),
            @event.EventType.Name,
            @event.GetHeaderString(MetadataHeaderNames.Initiator),
            @event.Sequence
        );
    }
}
