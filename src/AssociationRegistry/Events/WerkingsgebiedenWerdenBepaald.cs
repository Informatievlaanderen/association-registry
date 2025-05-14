namespace AssociationRegistry.Events;

public record WerkingsgebiedenWerdenBepaald(string VCode, Registratiedata.Werkingsgebied[] Werkingsgebieden) : IEvent
{

}



