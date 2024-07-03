namespace AssociationRegistry.Grar.Models;

using Events;

public record AddressMatchResponse(
    double Score,
    Registratiedata.AdresId? AdresId,
    string Adresvoorstelling,
    string Straatnaam,
    string Huisnummer,
    string Busnummer,
    string Postcode,
    string Gemeente
);

public record AddressDetailResponse(
    Registratiedata.AdresId AdresId,
    bool IsActief,
    string Adresvoorstelling,
    string Straatnaam,
    string Huisnummer,
    string Busnummer,
    string Postcode,
    string Gemeente
);
