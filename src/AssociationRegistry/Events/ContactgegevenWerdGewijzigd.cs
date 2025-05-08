namespace AssociationRegistry.Events;

public record ContactgegevenWerdGewijzigd(
    int ContactgegevenId,
    string Contactgegeventype,
    string Waarde,
    string Beschrijving,
    bool IsPrimair) : IEvent
{

}
