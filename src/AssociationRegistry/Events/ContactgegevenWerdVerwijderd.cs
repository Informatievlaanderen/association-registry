namespace AssociationRegistry.Events;


using Vereniging;

public record ContactgegevenWerdVerwijderd(int ContactgegevenId, string Type, string Waarde, string Beschrijving, bool IsPrimair) : IEvent
{

}
