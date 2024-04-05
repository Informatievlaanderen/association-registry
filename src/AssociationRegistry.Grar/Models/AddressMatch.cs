namespace AssociationRegistry.Grar.Models;

public record AddressMatch(
    double Score,
    string AdresId,
    AdresStatus? AdresStatus,
    string Straatnaam,
    string Huisnummer,
    string Busnummer,
    string Postcode,
    string Gemeentenaam
);
