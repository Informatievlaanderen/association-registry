namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using System.Linq;
using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Schema.Detail;

public static class PubliekVerenigingDetailMapper
{
    public static PubliekVerenigingDetailResponse Map(PubliekVerenigingDetailDocument document, AppSettings appSettings)
        => new()
        {
            Context = $"{appSettings.BaseUrl}/v1/contexten/detail-vereniging-context.json",
            Vereniging = new Vereniging
            {
                VCode = document.VCode,
                Type = Map(document.Type),
                Naam = document.Naam,
                KorteNaam = document.KorteNaam,
                KorteBeschrijving = document.KorteBeschrijving,
                Startdatum = document.Startdatum,
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
            Type = r.Type,
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
            Type = info.Type,
            Waarde = info.Waarde,
            Beschrijving = info.Beschrijving,
            IsPrimair = info.IsPrimair,
        };

    private static VerenigingsType Map(PubliekVerenigingDetailDocument.VerenigingsType verenigingsType)
        => new()
        {
            Code = verenigingsType.Code,
            Beschrijving = verenigingsType.Beschrijving,
        };

    private static Sleutel Map(PubliekVerenigingDetailDocument.Sleutel s)
        => new()
        {
            Bron = s.Bron,
            Waarde = s.Waarde,
        };

    private static HoofdactiviteitVerenigingsloket Map(PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket ha)
        => new() { Code = ha.Code, Beschrijving = ha.Beschrijving };

    private static Locatie Map(PubliekVerenigingDetailDocument.Locatie loc)
        => new()
        {
            Locatietype = loc.Locatietype,
            IsPrimair = loc.IsPrimair,
            Adresvoorstelling = loc.Adresvoorstelling,
            Naam = loc.Naam,
            Adres = new Adres
            {
                Straatnaam = loc.Adres.Straatnaam,
                Huisnummer = loc.Adres.Huisnummer,
                Busnummer = loc.Adres.Busnummer,
                Postcode = loc.Adres.Postcode,
                Gemeente = loc.Adres.Gemeente,
                Land = loc.Adres.Land,
            },
            AdresId = loc.AdresId is not null
                ? new AdresId
                {
                    Broncode = loc.AdresId.Broncode,
                    Bronwaarde = loc.AdresId.Bronwaarde,
                }
                : null,

        };
}
