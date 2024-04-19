﻿namespace AssociationRegistry.Grar.Models;

using Events;

public record AddressMatchResponse(
    double Score,
    Registratiedata.AdresId? AdresId,
    AdresStatus? AdresStatus,
    string Adresvoorstelling,
    string Straatnaam,
    string Huisnummer,
    string Busnummer,
    string Postcode,
    string Gemeente
);
