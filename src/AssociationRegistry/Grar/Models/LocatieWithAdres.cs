namespace AssociationRegistry.Grar.Models;

using Events;

public record LocatieWithAdres(int LocatieId, AddressDetailResponse Adres);
public record LocatieIdWithAdresId(int LocatieId, string AddressId);
