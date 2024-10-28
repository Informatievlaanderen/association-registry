namespace AssociationRegistry.Events;

using Grar.Models;
using Grar.Models.PostalInfo;
using Postnaam = Grar.Models.PostalInfo.Postnaam;

public static class GemeentenaamDecorator
{
    public static string DecorateGemeentenaam(
        string origineleGemeentenaam,
        PostalInformationResponse? postalInformationResponse,
        string gemeentenaamUitGrar)
    {
        if (postalInformationResponse is null) return gemeentenaamUitGrar;

        var origineleGemeenteNaamClean = GemeenteNaamSuffixCleanerRegex.Instance.Clean(origineleGemeentenaam);

        if (postalInformationResponse.Postnamen.HasSinglePostnaam)
        {
            if (postalInformationResponse.Postnamen.Single().IsEquivalentTo(postalInformationResponse.Gemeentenaam))
            {
                // Gemeentenaam reeds hoofdgemeente, correcte schrijfwijze en hoofdletters overnemen
                return postalInformationResponse.Gemeentenaam;
            }
            else
            {
                // Gemeentenaam geen hoofdgemeente, maar wel binnen de postnaam (gebruik deelgemeente syntax)
                return $"{postalInformationResponse.Postnamen.Single().Value} ({postalInformationResponse.Gemeentenaam})";
            }
        }

        var postNaam = postalInformationResponse.Postnamen.FindSingleWithGemeentenaam(origineleGemeenteNaamClean);

        var origineleGemeentenaamKomtVoorInPostalInformationResult = postNaam is not null;

        if (origineleGemeentenaamKomtVoorInPostalInformationResult)
        {
            // Gemeentenaam komt voor in de postnamen
            if (postNaam.IsEquivalentTo(postalInformationResponse.Gemeentenaam))
            {
                // Gemeentenaam reeds hoofdgemeente, correcte schrijfwijze en hoofdletters overnemen
                return postalInformationResponse.Gemeentenaam;
            }
            else
            {
                // Gemeentenaam geen hoofdgemeente, maar wel binnen de postnaam (gebruik deelgemeente syntax)
                return $"{postNaam.Value} ({postalInformationResponse.Gemeentenaam})";
            }
        }
        else
        {
            // Hoofdgemeente overnemen, postcode wint altijd
            return postalInformationResponse.Gemeentenaam;
        }
    }
}

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
                "België"),
        };

    public Registratiedata.AdresId AdresId { get; init; }
    public Registratiedata.Adres Adres { get; init; }

    public AdresMatchUitAdressenregister WithGemeentenaam(string gemeentenaam)
    {
        return this with
        {
            Adres = Adres with { Gemeente = gemeentenaam },
        };
    }
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
