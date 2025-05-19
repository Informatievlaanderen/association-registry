namespace AssociationRegistry.Events;

public record GeotagsWerdenBepaald(string VCode, Registratiedata.Geotag[] Geotags) : IEvent
{

}



