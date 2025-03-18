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
) : IAddressResponse
{
    public const double PerfectScore = 100;
};

public record AddressDetailResponse(
    Registratiedata.AdresId AdresId,
    bool IsActief,
    string Adresvoorstelling,
    string Straatnaam,
    string Huisnummer,
    string Busnummer,
    string Postcode,
    string Gemeente
) : IAddressResponse;

public interface IAddressResponse
{
    Registratiedata.AdresId? AdresId { get; }
    string Adresvoorstelling { get; }
    string Straatnaam { get; }
    string Huisnummer { get; }
    string Busnummer { get; }
    string Postcode { get; }
    string Gemeente { get; }
}
