namespace AssociationRegistry.GemeentenaamVerrijking;

using DecentraalBeheer.Vereniging.Adressen;
using Grar.Models.PostalInfo;
using Vereniging;

public static class GemeentenaamDecorator
{
    public static VerrijkteGemeentenaam VerrijkGemeentenaam(
        Gemeentenaam gemeentenaam,
        PostalInfoDetailResponse? postalInformationResponse,
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
        PostalInfoDetailResponse? postalInformationResponse,
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
