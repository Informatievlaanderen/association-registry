namespace AssociationRegistry.Events;

using Grar.Models;
using System.Text.RegularExpressions;

public record AdresMatchUitAdressenregister
{
    public static AdresMatchUitAdressenregister FromResponse(AddressMatchResponse response)
        => new()
        {
            AdresId = response.AdresId,
            Adres = new Registratiedata.Adres(
                response.Straatnaam,
                response.Huisnummer,
                response.Busnummer,
                response.Postcode,
                response.Gemeente,
                "België")
        };

    public AdresMatchUitAdressenregister WithPostalInformation(string origineleGemeentenaam, PostalInformationResponse? postalInformationResponse)
    {
        if (postalInformationResponse is null) return this;

        var origineleGemeenteNaamClean = GemeentenaamSuffixCleaner.RemovePartsBetweenBraces(origineleGemeentenaam);

        if (postalInformationResponse.HasSinglePostnaam)
        {
            return BepaalGemeentenaam(postalInformationResponse);
        }

        var postNaam = postalInformationResponse.GetPostnaamWhenEqualsGemeentenaam(origineleGemeenteNaamClean);

        if (postNaam is not null)
        {
            return BepaalGemeentenaam(postalInformationResponse, postNaam);
        }

        // Hoofdgemeente overnemen, postcode wint altijd
        return this with {
            Adres = Adres with { Gemeente = postalInformationResponse.Gemeentenaam }
        };
    }

    private AdresMatchUitAdressenregister BepaalGemeentenaam(PostalInformationResponse postalInformationResponse)
    {
        if (string.Equals(postalInformationResponse.Gemeentenaam, postalInformationResponse.Postnamen.Single(),
                          StringComparison.CurrentCultureIgnoreCase))
        {
            // Gemeentenaam reeds hoofdgemeente, correcte schrijfwijze en hoofdletters overnemen
            return this with {
                Adres = Adres with { Gemeente = postalInformationResponse.Gemeentenaam }
            };
        }

        // Gemeentenaam geen hoofdgemeente, maar wel binnen de postnaam (gebruik deelgemeente syntax)
        return this with {
            Adres = Adres with { Gemeente = $"{postalInformationResponse.Postnamen.Single()} ({postalInformationResponse.Gemeentenaam})" }
        };
    }

    private AdresMatchUitAdressenregister BepaalGemeentenaam(PostalInformationResponse postalInformationResponse, string? postNaam)
    {
        // Gemeentenaam komt voor in de postnamen
        if (postalInformationResponse.HasGemeentenaam(postNaam))
        {
            // Gemeentenaam reeds hoofdgemeente, correcte schrijfwijze en hoofdletters overnemen
            return this with {
                Adres = Adres with { Gemeente = postalInformationResponse.Gemeentenaam }
            };
        }

        // Gemeentenaam geen hoofdgemeente, maar wel binnen de postnaam (gebruik deelgemeente syntax)
        return this with {
            Adres = Adres with { Gemeente = $"{postNaam} ({postalInformationResponse.Gemeentenaam})" }
        };
    }

    public Registratiedata.AdresId? AdresId { get; init; }
    public Registratiedata.Adres Adres { get; init; }
}

public record NietUniekeAdresMatchUitAdressenregister
{
    public static NietUniekeAdresMatchUitAdressenregister FromResponse(AddressMatchResponse response)
        => new()
        {
            Score = response.Score,
            AdresId = response.AdresId,
            Adresvoorstelling = response.Adresvoorstelling,

        };

    public Registratiedata.AdresId? AdresId { get; init; }
    public string Adresvoorstelling { get; init; }
    public double Score { get; init; }
}
