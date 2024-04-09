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

    public AdresMatchUitGrar DecorateWithPostalInformation(PostalInformationResponse? postalInformationResponse)
    {
        if (postalInformationResponse is null) return this;

        var postNaam =
            postalInformationResponse.Postnamen.SingleOrDefault(
                sod => sod.Equals(Gemeentenaam, StringComparison.InvariantCultureIgnoreCase));

        if (postNaam is not null)
        {
            // Gemeentenaam komt voor in de postnamen
            if (postalInformationResponse.Gemeentenaam.Equals(postNaam, StringComparison.InvariantCultureIgnoreCase))
            {
                // Gemeentenaam reeds hoofdgemeente, correcte schrijfwijze en hoofdletters overnemen
                return this with { Gemeentenaam = postalInformationResponse.Gemeentenaam };
            }
            else
            {
                // Gemeentenaam geen hoofdgemeente, maar wel binnen de postnaam (gebruik deelgemeente syntax)
                return this with { Gemeentenaam = $"{postNaam} ({postalInformationResponse.Gemeentenaam})" };
            }
        }
        else
        {
            // Hoofdgemeente overnemen, postcode wint altijd
            return this with { Gemeentenaam = postalInformationResponse.Gemeentenaam };
        }
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
