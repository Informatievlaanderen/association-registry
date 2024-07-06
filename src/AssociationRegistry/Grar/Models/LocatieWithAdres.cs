namespace AssociationRegistry.Grar.Models;

public record LocatieWithAdres(int LocatieId, AddressDetailResponse? Adres);
public record LocatieIdWithAdresId(int LocatieId, string AddressId);
