namespace AssociationRegistry.Events;

public record DubbeleVerenigingenWerdenGedetecteerd(string Key, string Naam, Registratiedata.Locatie[] Locaties, Registratiedata.DuplicaatVereniging[] GedetecteerdeDubbels) : IEvent
{

}
