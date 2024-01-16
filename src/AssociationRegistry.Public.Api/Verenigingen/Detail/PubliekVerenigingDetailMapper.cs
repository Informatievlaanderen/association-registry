namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Schema.Detail;
using System.Linq;

public static class PubliekVerenigingDetailMapper
{
    public static PubliekVerenigingDetailResponse Map(PubliekVerenigingDetailDocument document, AppSettings appSettings)
        => new()
        {
            Context = $"{appSettings.BaseUrl}/v1/contexten/publiek/detail-vereniging-context.json",
            Vereniging = new Vereniging
            {
                id = document.JsonLdMetadata.Id,
                type = document.JsonLdMetadata.Type,

                VCode = document.VCode,
                Verenigingstype = Map(document.Verenigingstype),
                Naam = document.Naam,
                Roepnaam = document.Roepnaam,
                KorteNaam = document.KorteNaam,
                KorteBeschrijving = document.KorteBeschrijving,
                Startdatum = document.Startdatum,
                Doelgroep = new DoelgroepResponse
                {
                    Minimumleeftijd = document.Doelgroep.Minimumleeftijd,
                    Maximumleeftijd = document.Doelgroep.Maximumleeftijd,
                },
                Status = document.Status,
                Contactgegevens = document.Contactgegevens.Select(Map).ToArray(),
                Locaties = document.Locaties.Select(Map).ToArray(),
                HoofdactiviteitenVerenigingsloket = document.HoofdactiviteitenVerenigingsloket.Select(Map).ToArray(),
                Sleutels = document.Sleutels.Select(Map).ToArray(),
                Relaties = document.Relaties.Select(r => Map(appSettings, r)).ToArray(),
            },
            Metadata = new Metadata { DatumLaatsteAanpassing = document.DatumLaatsteAanpassing },
        };

    private static Relatie Map(AppSettings appSettings, PubliekVerenigingDetailDocument.Relatie r)
        => new()
        {
            Relatietype = r.Relatietype,
            AndereVereniging = new Relatie.GerelateerdeVereniging
            {
                KboNummer = r.AndereVereniging.KboNummer,
                VCode = r.AndereVereniging.VCode,
                Naam = r.AndereVereniging.Naam,
                Detail = !string.IsNullOrEmpty(r.AndereVereniging.VCode)
                    ? $"{appSettings.BaseUrl}/v1/verenigingen/{r.AndereVereniging.VCode}"
                    : string.Empty,
            },
        };

    private static Contactgegeven Map(PubliekVerenigingDetailDocument.Contactgegeven info)
        => new()
        {
            id = info.JsonLdMetadata.Id,
            type = info.JsonLdMetadata.Type,
            Contactgegeventype = info.Contactgegeventype,
            Waarde = info.Waarde,
            Beschrijving = info.Beschrijving,
            IsPrimair = info.IsPrimair,
        };

    private static VerenigingsType Map(PubliekVerenigingDetailDocument.VerenigingsType verenigingsType)
        => new()
        {
            Code = verenigingsType.Code,
            Naam = verenigingsType.Naam,
        };

    private static Sleutel Map(PubliekVerenigingDetailDocument.Sleutel s)
        => new()
        {
            id = s.JsonLdMetadata.Id,
            type = s.JsonLdMetadata.Type,

            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                id = s.GestructureerdeIdentificator.JsonLdMetadata.Id,
                type = s.GestructureerdeIdentificator.JsonLdMetadata.Type,
                Nummer = s.GestructureerdeIdentificator.Nummer,
            },

            Bron = s.Bron,
            Waarde = s.Waarde,
        };

    private static HoofdactiviteitVerenigingsloket Map(PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket ha)
        => new()
        {
            id = ha.JsonLdMetadata.Id,
            type = ha.JsonLdMetadata.Type,
            Code = ha.Code,
            Naam = ha.Naam,
        };

    private static Locatie Map(PubliekVerenigingDetailDocument.Locatie loc)
        => new()
        {
            id = loc.JsonLdMetadata.Id,
            type = loc.JsonLdMetadata.Type,
            Locatietype = loc.Locatietype,
            IsPrimair = loc.IsPrimair,
            Adresvoorstelling = loc.Adresvoorstelling,
            Naam = loc.Naam,
            Adres = Map(loc.Adres),
            AdresId = Map(loc.AdresId),
        };

    private static AdresId? Map(PubliekVerenigingDetailDocument.AdresId? adresId)
        => adresId is not null
            ? new AdresId
            {
                Broncode = adresId.Broncode,
                Bronwaarde = adresId.Bronwaarde,
            }
            : null;

    private static Adres? Map(PubliekVerenigingDetailDocument.Adres? adres)
        => adres is not null
            ? new Adres
            {
                id = adres.JsonLdMetadata.Id,
                type = adres.JsonLdMetadata.Type,
                Straatnaam = adres.Straatnaam,
                Huisnummer = adres.Huisnummer,
                Busnummer = adres.Busnummer,
                Postcode = adres.Postcode,
                Gemeente = adres.Gemeente,
                Land = adres.Land,
            }
            : null;
}
