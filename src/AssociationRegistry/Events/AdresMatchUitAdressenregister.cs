﻿namespace AssociationRegistry.Events;

using Grar.Models;
using Grar.Models.PostalInfo;
using Postnaam = Grar.Models.PostalInfo.Postnaam;

public static class GemeentenaamDecorator
{
    public static VerrijkteGemeentenaam VerrijkGemeentenaam(
        string origineleGemeentenaam,
        PostalInformationResponse? postalInformationResponse,
        string gemeentenaamUitGrar)
    {
        if (postalInformationResponse is null) return VerrijkteGemeentenaam.ZonderPostnaam(gemeentenaamUitGrar);

        var origineleGemeenteNaamClean = GemeenteNaamSuffixCleanerRegex.Instance.Clean(origineleGemeentenaam);

        var postnaam = postalInformationResponse.Postnamen.FindSingle() ??
                       postalInformationResponse.Postnamen.FindSingleWithGemeentenaam(origineleGemeenteNaamClean);

        if (postnaam is not null)
        {
            if (postnaam.IsEquivalentTo(postalInformationResponse.Gemeentenaam))
            {
                // Gemeentenaam reeds hoofdgemeente, correcte schrijfwijze en hoofdletters overnemen
                return VerrijkteGemeentenaam.ZonderPostnaam(postalInformationResponse.Gemeentenaam);
            }

            // Gemeentenaam geen hoofdgemeente, maar wel binnen de postnaam (gebruik deelgemeente syntax)
            return VerrijkteGemeentenaam.MetPostnaam(postnaam, postalInformationResponse.Gemeentenaam);
        }

        // Hoofdgemeente overnemen, postcode wint altijd
        return VerrijkteGemeentenaam.ZonderPostnaam(postalInformationResponse.Gemeentenaam);
    }
}

public record VerrijkteGemeentenaam
{
    public Postnaam? Postnaam { get; }
    public string Gemeentenaam { get; }

    private VerrijkteGemeentenaam(Postnaam? postnaam, string gemeentenaam)
    {
        Postnaam = postnaam;
        Gemeentenaam = gemeentenaam;
    }

    public static VerrijkteGemeentenaam ZonderPostnaam(string gemeentenaam)
        => new(null, gemeentenaam);

    public static VerrijkteGemeentenaam MetPostnaam(Postnaam postnaam, string gemeentenaam)
    {
        return new(postnaam, gemeentenaam);
    }

    public string Format()
    {
        if(Postnaam is not null)
            return $"{Postnaam.Value} ({Gemeentenaam})";

        return Gemeentenaam;
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
