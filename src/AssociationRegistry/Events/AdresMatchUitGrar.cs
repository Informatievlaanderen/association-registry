namespace AssociationRegistry.Events;

using Grar.Models;

public record AdresMatchUitGrar
{
    public static AdresMatchUitGrar FromResponse(AddressMatchResponse response)
        => new()
        {
            AdresId = response.AdresId,
            AdresStatus = response.AdresStatus,
            Score = response.Score,
            Straatnaam = response.Straatnaam,
            Huisnummer = response.Huisnummer,
            Busnummer = response.Busnummer,
            Postcode = response.Postcode,
            Gemeentenaam = response.Gemeentenaam
        };

    public AdresMatchUitGrar DecorateWithPostalInformation(PostalInformationResponse postalInformationResponse)
    {

        return this;
    }

    public string AdresId { get; init; }
    public AdresStatus? AdresStatus { get; init; }
    public double Score { get; init; }
    public string Straatnaam { get; init; }
    public string Huisnummer { get; init; }
    public string Busnummer { get; init; }
    public string Postcode { get; init; }
    public string Gemeentenaam { get; private set; }
    public string Land { get; init; } = "België";
}
