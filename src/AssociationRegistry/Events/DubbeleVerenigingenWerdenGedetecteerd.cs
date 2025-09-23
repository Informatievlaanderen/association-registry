namespace AssociationRegistry.Events;

public record DubbeleVerenigingenWerdenGedetecteerd(string BevestigingstokenKey, string Bevestigingstoken, string Naam, Registratiedata.Locatie[] Locaties, Registratiedata.DuplicateVereniging[] GedetecteerdeDubbels) : IEvent
{

}
