namespace AssociationRegistry.Events;

using Framework;

public record AdresWerdOvergenomenUitAdressenregister(string VCode, int LocatieId, AdresMatchUitAdressenregister OvergenomenAdresUitAdressenregister) : IEvent;
