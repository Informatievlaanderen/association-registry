namespace AssociationRegistry.Events;

using Framework;

public record AdresWerdGeheradresseerdInAdressenregister(
    string VCode,
    int LocatieId,
    AdresDetailUitAdressenregister adres,
    string idempotenceKey) : IEvent;
