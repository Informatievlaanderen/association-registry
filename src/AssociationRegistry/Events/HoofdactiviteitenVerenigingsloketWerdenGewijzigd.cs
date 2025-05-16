namespace AssociationRegistry.Events;


using Vereniging;

public record HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent
{

}
