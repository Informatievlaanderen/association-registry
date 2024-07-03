namespace AssociationRegistry.Events;

using Framework;

public record AdresWerdOntkoppeldVanAdressenregister(string VCode, int LocatieId) : IEvent;
