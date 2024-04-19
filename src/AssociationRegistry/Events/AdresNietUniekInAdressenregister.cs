namespace AssociationRegistry.Events;

using Framework;

public record AdresNietUniekInAdressenregister(string VCode, int LocatieId, NietUniekeAdresMatchUitAdressenregister[] NietOvergenomenAdressenUitAdressenregister) : IEvent;
