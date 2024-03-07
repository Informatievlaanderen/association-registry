namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VerenigingWerdGestoptInKBO(DateOnly Einddatum) : IEvent
{
    public static VerenigingWerdGestoptInKBO With(Datum einddatum)
        => new(einddatum.Value);
}
