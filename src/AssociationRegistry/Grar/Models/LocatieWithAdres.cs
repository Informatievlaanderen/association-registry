namespace AssociationRegistry.Grar.Models;

public record LocatieWithAdres(int LocatieId, AddressDetailResponse Address);
public record LocatieIdWithAdresId(int LocatieId, string AddressId);

