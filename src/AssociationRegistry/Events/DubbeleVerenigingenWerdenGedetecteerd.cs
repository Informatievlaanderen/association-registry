namespace AssociationRegistry.Events;

public record DubbeleVerenigingenWerdenGedetecteerd(string DDCode, string Naam, Registratiedata.Locatie[] Locaties, Registratiedata.DuplicaatVereniging[] GedetecteerdeDubbels) : IEvent
{

}
