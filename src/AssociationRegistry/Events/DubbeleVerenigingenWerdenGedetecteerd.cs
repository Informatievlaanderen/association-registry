namespace AssociationRegistry.Events;

public record DubbeleVerenigingenWerdenGedetecteerd(string Bevestigingstoken, string Naam, Registratiedata.Locatie[] Locaties, Registratiedata.DuplicateVereniging[] GedetecteerdeDubbels) : IEvent
{

}
