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
                "België"),
        };

    public class GemeenteNaamSuffixCleanerRegex
    {
        private readonly Regex _regex;
        private const string RemoveBracketsAndContent = "\\(.*?\\)";

        public GemeenteNaamSuffixCleanerRegex()
        {
           _regex = new Regex(RemoveBracketsAndContent);
        }

        public string Clean(string gemeentenaam)
            => _regex.Replace(gemeentenaam, "").Trim();
    }

    // hekelgem (affligem)
    public AdresMatchUitAdressenregister DecorateWithPostalInformation(string origineleGemeentenaam, PostalInformationResponse? postalInformationResponse)
    {
        if (postalInformationResponse is null) return this;

        var origineleGemeenteNaamClean = new Regex("\\(.*?\\)").Replace(origineleGemeentenaam, "").Trim();

        if (postalInformationResponse.Postnamen.Length == 1)
        {
            if (string.Equals(postalInformationResponse.Gemeentenaam, postalInformationResponse.Postnamen.Single(),
                              StringComparison.CurrentCultureIgnoreCase))
            {
                // Gemeentenaam reeds hoofdgemeente, correcte schrijfwijze en hoofdletters overnemen
                return this with {
                    Adres = Adres with { Gemeente = postalInformationResponse.Gemeentenaam },
                };
            }
            else
            {
                // Gemeentenaam geen hoofdgemeente, maar wel binnen de postnaam (gebruik deelgemeente syntax)
                return this with {
                    Adres = Adres with { Gemeente = $"{postalInformationResponse.Postnamen.Single()} ({postalInformationResponse.Gemeentenaam})" },
                };
            }
        }

        var postNaam =
            postalInformationResponse.Postnamen.SingleOrDefault(
                sod => sod.Equals(origineleGemeenteNaamClean, StringComparison.InvariantCultureIgnoreCase));
        var origineleGemeentenaamKomtVoorInPostalInformationResult = postNaam is not null;

        if (origineleGemeentenaamKomtVoorInPostalInformationResult)
        {
            // Gemeentenaam komt voor in de postnamen
            if (postalInformationResponse.Gemeentenaam.Equals(postNaam, StringComparison.InvariantCultureIgnoreCase))
            {
                // Gemeentenaam reeds hoofdgemeente, correcte schrijfwijze en hoofdletters overnemen
                return this with {
                    Adres = Adres with { Gemeente = postalInformationResponse.Gemeentenaam },
                };
            }
            else
            {
                // Gemeentenaam geen hoofdgemeente, maar wel binnen de postnaam (gebruik deelgemeente syntax)
                return this with {
                    Adres = Adres with { Gemeente = $"{postNaam} ({postalInformationResponse.Gemeentenaam})" },
                };
            }
        }
        else
        {
            // Hoofdgemeente overnemen, postcode wint altijd
            return this with {
                Adres = Adres with { Gemeente = postalInformationResponse.Gemeentenaam },
            };
        }
    }

    public Registratiedata.AdresId AdresId { get; init; }
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
