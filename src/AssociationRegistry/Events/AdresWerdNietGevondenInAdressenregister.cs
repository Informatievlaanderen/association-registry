namespace AssociationRegistry.Events;

using Framework;

public record AdresWerdNietGevondenInAdressenregister(string VCode, int LocatieId) : IEvent;
