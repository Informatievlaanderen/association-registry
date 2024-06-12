namespace AssociationRegistry.Events;

using Framework;

public record AdresWerdGewijzigdInAdressenregister(
    string VCode,
    int LocatieId,
    AdresDetailUitAdressenregister AdresDetailUitAdressenregister,
    string IdempotenceKey) : IEvent;
