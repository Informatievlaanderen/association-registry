namespace AssociationRegistry.Events;

public record HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent
{

}
