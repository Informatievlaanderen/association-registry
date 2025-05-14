namespace AssociationRegistry.Events;

public record WerkingsgebiedenWerdenGewijzigd(string VCode, Registratiedata.Werkingsgebied[] Werkingsgebieden) : IEvent
{

}
