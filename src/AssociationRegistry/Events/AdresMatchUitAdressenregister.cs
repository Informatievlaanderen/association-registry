namespace AssociationRegistry.Events;

using Grar.Models;
using Grar.Models.PostalInfo;
using Vereniging;
using Gemeentenaam = Vereniging.Gemeentenaam;
using Postnaam = Grar.Models.PostalInfo.Postnaam;

public static class GemeentenaamDecorator
{
    public static VerrijkteGemeentenaam VerrijkGemeentenaam(
        Gemeentenaam gemeentenaam,
        PostalInformationResponse? postalInformationResponse,
        string gemeentenaamUitGrar)
    {
        if (postalInformationResponse is null) return VerrijkteGemeentenaam.ZonderPostnaam(gemeentenaamUitGrar);

        var origineleGemeenteNaamClean = GemeenteNaamSuffixCleanerRegex.Instance.Clean(gemeentenaam);

        var postnaam = postalInformationResponse.Postnamen.FindSingleOrDefault() ??
                       postalInformationResponse.Postnamen.FindSingleWithGemeentenaam(origineleGemeenteNaamClean);

        if (postnaam is not null && !postnaam.IsEquivalentTo(postalInformationResponse.Gemeentenaam))
        {
            // Gemeentenaam geen hoofdgemeente, maar wel binnen de postnaam (gebruik deelgemeente syntax)
            return VerrijkteGemeentenaam.MetPostnaam(postnaam, postalInformationResponse.Gemeentenaam);
        }

        // Hoofdgemeente overnemen, postcode wint altijd
        return VerrijkteGemeentenaam.ZonderPostnaam(postalInformationResponse.Gemeentenaam);
    }

    public static VerrijkteGemeentenaam VerrijkGemeentenaam(
        PostalInformationResponse? postalInformationResponse,
        string gemeentenaamUitGrar)
    {
        if (postalInformationResponse is null) return VerrijkteGemeentenaam.ZonderPostnaam(gemeentenaamUitGrar);

        var postnaam = postalInformationResponse.Postnamen.FindSingleOrDefault();

        if (postnaam is not null && !postnaam.IsEquivalentTo(postalInformationResponse.Gemeentenaam))
        {
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
    {
        if (string.IsNullOrEmpty(gemeentenaam))
            throw new ArgumentException(nameof(gemeentenaam));

        return new VerrijkteGemeentenaam(null, gemeentenaam);
    }

    public static VerrijkteGemeentenaam MetPostnaam(Postnaam postnaam, string gemeentenaam)
    {
        if (string.IsNullOrEmpty(postnaam))
            throw new ArgumentException(nameof(postnaam));
        if (string.IsNullOrEmpty(gemeentenaam))
            throw new ArgumentException(nameof(gemeentenaam));

        return new(postnaam, gemeentenaam);
    }

    public string Format()
    {
        if(Postnaam is not null)
            return $"{Postnaam.Value} ({Gemeentenaam})";

        return Gemeentenaam;
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
