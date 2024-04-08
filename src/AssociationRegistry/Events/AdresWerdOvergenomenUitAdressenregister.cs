namespace AssociationRegistry.Events;

using Framework;

public record AdresWerdOvergenomenUitAdressenregister(string VCode, int LocatieId, AdresMatchUitGrar OvergenomenAdresUitGrar, AdresMatchUitGrar[] NietOvergenomenAdressenUitGrar) : IEvent;
