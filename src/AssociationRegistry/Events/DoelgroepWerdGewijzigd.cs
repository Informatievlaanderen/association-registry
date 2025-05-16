namespace AssociationRegistry.Events;


using Vereniging;

public record DoelgroepWerdGewijzigd(Registratiedata.Doelgroep Doelgroep) : IEvent
{

}
