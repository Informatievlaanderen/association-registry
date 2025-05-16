namespace AssociationRegistry.Events;


using Vereniging;

public record VerenigingWerdGestoptInKBO(DateOnly Einddatum) : IEvent
{

}
