namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Formats;
using Infrastructure;
using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Schema.Detail;
using Vereniging;
using Adres = ResponseModels.Adres;
using AdresId = ResponseModels.AdresId;
using Contactgegeven = ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = ResponseModels.Lidmaatschap;
using Locatie = ResponseModels.Locatie;
using Vereniging = ResponseModels.Vereniging;
using Verenigingssubtype = ResponseModels.Verenigingssubtype;
using Werkingsgebied = ResponseModels.Werkingsgebied;

public class PubliekVerenigingDetailMapper
{
    private readonly AppSettings _appSettings;
    private readonly IVerenigingstypeMapper _verenigingstypeMapper;
    private readonly string _version;

    public PubliekVerenigingDetailMapper(AppSettings appSettings, string version)
    {
        _appSettings = appSettings;
        _verenigingstypeMapper = version == WellknownVersions.V2 ? new VerenigingstypeMapperV2() : new VerenigingstypeMapperV1();
        _version = version;
    }

    public PubliekVerenigingDetailResponse Map(
        PubliekVerenigingDetailDocument document,
        INamenVoorLidmaatschapMapper lidmaatschapMapper)
        => new()
        {
            Context = $"{_appSettings.BaseUrl}/v1/contexten/publiek/detail-vereniging-context.json",
            Vereniging = new Vereniging
            {
                type = document.JsonLdMetadataType,
                VCode = document.VCode,
                Verenigingstype = _verenigingstypeMapper.Map<VerenigingsType, PubliekVerenigingDetailDocument.Types.Verenigingstype>(document.Verenigingstype),
                Verenigingssubtype = _verenigingstypeMapper.MapSubtype<Verenigingssubtype,  PubliekVerenigingDetailDocument.Types.Verenigingssubtype>(document.Verenigingssubtype),
                Naam = document.Naam,
                Roepnaam = document.Roepnaam,
                KorteNaam = document.KorteNaam,
                KorteBeschrijving = document.KorteBeschrijving,
                Startdatum = document.Startdatum,
                Doelgroep = new DoelgroepResponse
                {
                    id = document.Doelgroep.JsonLdMetadata.Id,
                    type = document.Doelgroep.JsonLdMetadata.Type,
                    Minimumleeftijd = document.Doelgroep.Minimumleeftijd,
                    Maximumleeftijd = document.Doelgroep.Maximumleeftijd,
                },
                Status = document.Status,
                Contactgegevens = document.Contactgegevens.Select(Map).ToArray(),
                Locaties = document.Locaties.Select(Map).ToArray(),
                HoofdactiviteitenVerenigingsloket = document.HoofdactiviteitenVerenigingsloket.Select(Map).ToArray(),
                Werkingsgebieden = document.Werkingsgebieden.Select(Map).ToArray(),
                Sleutels = document.Sleutels.Select(Map).ToArray(),
                Relaties = document.Relaties.Select(r => Map(_appSettings, r)).ToArray(),
                Lidmaatschappen = document.Lidmaatschappen.Select(l => Map(l, lidmaatschapMapper)).ToArray(),
            },
            Metadata = new Metadata { DatumLaatsteAanpassing = document.DatumLaatsteAanpassing },
        };

    private static ResponseModels.Verenigingssubtype Map(PubliekVerenigingDetailDocument.Types.Verenigingssubtype subtype)
        => new()
        {
            Code = subtype.Code,
            Naam = subtype.Naam,
        };

    private static Lidmaatschap Map(PubliekVerenigingDetailDocument.Types.Lidmaatschap l, INamenVoorLidmaatschapMapper namenVoorLidmaatschapMapper)
        => new()
        {
            id = l.JsonLdMetadata.Id,
            type = l.JsonLdMetadata.Type,
            Beschrijving = l.Beschrijving,
            Naam = namenVoorLidmaatschapMapper.MapNaamVoorLidmaatschap(l.AndereVereniging),
            AndereVereniging = l.AndereVereniging,
            Identificatie = l.Identificatie,
            Van = l.Van.FormatAsBelgianDate(),
            Tot = l.Tot.FormatAsBelgianDate(),
        };

    private static Relatie Map(AppSettings appSettings, PubliekVerenigingDetailDocument.Types.Relatie r)
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

    private static Contactgegeven Map(PubliekVerenigingDetailDocument.Types.Contactgegeven info)
        => new()
        {
            id = info.JsonLdMetadata.Id,
            type = info.JsonLdMetadata.Type,
            Contactgegeventype = info.Contactgegeventype,
            Waarde = info.Waarde,
            Beschrijving = info.Beschrijving,
            IsPrimair = info.IsPrimair,
        };

    private static VerenigingsType Map(PubliekVerenigingDetailDocument.Types.Verenigingstype verenigingstype)
        => new()
        {
            Code = verenigingstype.Code,
            Naam = verenigingstype.Naam,
        };

    private static Sleutel Map(PubliekVerenigingDetailDocument.Types.Sleutel s)
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

            CodeerSysteem = s.CodeerSysteem,
            Bron = s.Bron,
            Waarde = s.Waarde,
        };

    private static HoofdactiviteitVerenigingsloket Map(PubliekVerenigingDetailDocument.Types.HoofdactiviteitVerenigingsloket ha)
        => new()
        {
            id = ha.JsonLdMetadata.Id,
            type = ha.JsonLdMetadata.Type,
            Code = ha.Code,
            Naam = ha.Naam,
        };

    private static Werkingsgebied Map(PubliekVerenigingDetailDocument.Types.Werkingsgebied wg)
        => new()
        {
            id = wg.JsonLdMetadata.Id,
            type = wg.JsonLdMetadata.Type,
            Code = wg.Code,
            Naam = wg.Naam,
        };

    private static Locatie Map(PubliekVerenigingDetailDocument.Types.Locatie loc)
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
            VerwijstNaar = Map(loc.VerwijstNaar),
        };

    private static AdresVerwijzing? Map(PubliekVerenigingDetailDocument.Types.Locatie.AdresVerwijzing? verwijzing)
    {
        if (verwijzing is null) return null;

        return new AdresVerwijzing
        {
            id = verwijzing.JsonLdMetadata.Id,
            type = verwijzing.JsonLdMetadata.Type,
        };
    }

    private static AdresId? Map(PubliekVerenigingDetailDocument.Types.AdresId? adresId)
        => adresId is not null
            ? new AdresId
            {
                Broncode = adresId.Broncode,
                Bronwaarde = adresId.Bronwaarde,
            }
            : null;

    private static Adres? Map(PubliekVerenigingDetailDocument.Types.Adres? adres)
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

public interface INamenVoorLidmaatschapMapper
{
    string MapNaamVoorLidmaatschap(string vCode);
}

public class VerplichteNamenVoorLidmaatschapMapper : INamenVoorLidmaatschapMapper
{
    private readonly Dictionary<string, string> _namenVoorLidmaatschap;

    public VerplichteNamenVoorLidmaatschapMapper(Dictionary<string, string> namenVoorLidmaatschap)
    {
        _namenVoorLidmaatschap = namenVoorLidmaatschap;
    }

    public string MapNaamVoorLidmaatschap(string vCode)
        => _namenVoorLidmaatschap[vCode];
}

