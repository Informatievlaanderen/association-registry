namespace AssociationRegistry.Events;

public record ContactgegevenWerdVerwijderd(int ContactgegevenId, string Type, string Waarde, string Beschrijving, bool IsPrimair) : IEvent
{

}
