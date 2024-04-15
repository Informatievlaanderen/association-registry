namespace AssociationRegistry.Events;

using Grar.Models;

public record AdresMatchUitGrar
{
    public static AdresMatchUitGrar FromResponse(AddressMatchResponse response)
        => new()
        {
            Score = response.Score,
            AdresStatus = response.AdresStatus,
            AdresId = response.AdresId,
            Adres = new Registratiedata.Adres(
                response.Straatnaam,
                response.Huisnummer,
                response.Busnummer,
                response.Postcode,
                response.Gemeentenaam,
                "België")
        };

    public AdresMatchUitGrar DecorateWithPostalInformation(string origineleGemeentenaam, PostalInformationResponse? postalInformationResponse)
    {
        if (postalInformationResponse is null) return this;

        var postNaam =
            postalInformationResponse.Postnamen.SingleOrDefault(
                sod => sod.Equals(origineleGemeentenaam, StringComparison.InvariantCultureIgnoreCase));
        var origineleGemeentenaamKomtVoorInPostalInformationResult = postNaam is not null;

        if (origineleGemeentenaamKomtVoorInPostalInformationResult)
        {
            // Gemeentenaam komt voor in de postnamen
            if (postalInformationResponse.Gemeentenaam.Equals(postNaam, StringComparison.InvariantCultureIgnoreCase))
            {
                // Gemeentenaam reeds hoofdgemeente, correcte schrijfwijze en hoofdletters overnemen
                return this with {
                    Adres = Adres with { Gemeente = postalInformationResponse.Gemeentenaam }
                };
            }
            else
            {
                // Gemeentenaam geen hoofdgemeente, maar wel binnen de postnaam (gebruik deelgemeente syntax)
                return this with {
                    Adres = Adres with { Gemeente = $"{postNaam} ({postalInformationResponse.Gemeentenaam})" }
                };
            }
        }
        else
        {
            // Hoofdgemeente overnemen, postcode wint altijd
            return this with {
                Adres = Adres with { Gemeente = postalInformationResponse.Gemeentenaam }
            };
        }
    }

    public Registratiedata.AdresId? AdresId { get; init; }
    public Registratiedata.Adres Adres { get; init; }
    public AdresStatus? AdresStatus { get; init; }
    public double Score { get; init; }
}
