namespace AssociationRegistry.CommandHandling.Grar.NightlyAdresSync.SyncAdresLocaties;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;

public record SyncAdresLocatiesCommand(string VCode, List<LocatieWithAdres> LocatiesWithAdres, string IdempotenceKey);

public record AddressDetailResponse(
    AdresId AdresId,
    bool IsActief,
    string Adresvoorstelling,
    string Straatnaam,
    string Huisnummer,
    string Busnummer,
    string Postcode,
    string Gemeente);

public record AdresId(string Broncode, string Bronwaarde);
