namespace AssociationRegistry.Events;

public record MaatschappelijkeZetelWerdVerwijderdUitKbo(
    Registratiedata.Locatie Locatie) : IEvent
{

}
