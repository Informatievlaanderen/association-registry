namespace AssociationRegistry.Events;


using Vereniging;

public record WerkingsgebiedenWerdenGewijzigd(string VCode, Registratiedata.Werkingsgebied[] Werkingsgebieden) : IEvent
{

}
