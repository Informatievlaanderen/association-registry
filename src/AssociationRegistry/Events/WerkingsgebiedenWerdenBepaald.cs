namespace AssociationRegistry.Events;


using Vereniging;

public record WerkingsgebiedenWerdenBepaald(string VCode, Registratiedata.Werkingsgebied[] Werkingsgebieden) : IEvent
{

}



