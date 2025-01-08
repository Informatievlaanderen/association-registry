namespace AssociationRegistry.Events;



public record AdresNietUniekInAdressenregister(string VCode, int LocatieId, NietUniekeAdresMatchUitAdressenregister[] NietOvergenomenAdressenUitAdressenregister) : IEvent;
