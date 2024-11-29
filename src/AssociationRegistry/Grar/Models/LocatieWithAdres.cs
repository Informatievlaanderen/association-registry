namespace AssociationRegistry.Grar.Models;

public record LocatieWithAdres(int LocatieId, AddressDetailResponse? Adres);
public record TeHeradresserenLocatie(int LocatieId, string DestinationAdresId);
