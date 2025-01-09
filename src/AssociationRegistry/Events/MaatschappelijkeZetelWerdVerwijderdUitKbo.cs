namespace AssociationRegistry.Events;


using Vereniging;

public record MaatschappelijkeZetelWerdVerwijderdUitKbo(
    Registratiedata.Locatie Locatie) : IEvent
{

}
