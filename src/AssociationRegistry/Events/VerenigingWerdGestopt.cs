namespace AssociationRegistry.Events;


using Vereniging;

public record VerenigingWerdGestopt(DateOnly Einddatum) : IEvent
{

}
