namespace AssociationRegistry.Events;


using Vereniging;

public record ContactgegevenWerdGewijzigd(
    int ContactgegevenId,
    string Contactgegeventype,
    string Waarde,
    string Beschrijving,
    bool IsPrimair) : IEvent
{

}
