﻿namespace AssociationRegistry.Events;

using Grar.Models;
using System.Text.RegularExpressions;

public record AdresDetailUitAdressenregister
{
    public static AdresDetailUitAdressenregister FromResponse(AddressDetailResponse response)
        => new()
        {
            AdresId = response.AdresId,
            Adres = new Registratiedata.AdresUitAdressenregister(
                response.Straatnaam,
                response.Huisnummer,
                response.Busnummer,
                response.Postcode,
                response.Gemeente),
        };

     public AdresDetailUitAdressenregister DecorateWithPostalInformation(string origineleGemeentenaam, PostalInformationResponse? postalInformationResponse)
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
    public Registratiedata.AdresUitAdressenregister Adres { get; init; }
}
