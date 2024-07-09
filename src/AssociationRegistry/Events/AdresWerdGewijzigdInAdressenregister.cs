namespace AssociationRegistry.Events;

using Framework;

public record AdresWerdGewijzigdInAdressenregister(
    string VCode,
    int LocatieId,
    Registratiedata.AdresId AdresId,
    Registratiedata.AdresUitAdressenregister Adres,
    string IdempotenceKey) : IEvent;
