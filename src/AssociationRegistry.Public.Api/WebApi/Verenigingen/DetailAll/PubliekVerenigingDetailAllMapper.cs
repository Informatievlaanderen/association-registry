namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.DetailAll;

using AssociationRegistry.Formats;
using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Schema.Detail;
using AssociationRegistry.Vereniging.Mappers;
using ResponseModels;
using Adres = ResponseModels.Adres;
using AdresId = ResponseModels.AdresId;
using Contactgegeven = ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = ResponseModels.Lidmaatschap;
using Locatie = ResponseModels.Locatie;
using SubverenigingVan = ResponseModels.SubverenigingVan;
using Vereniging = ResponseModels.Vereniging;
using Verenigingssubtype = ResponseModels.Verenigingssubtype;
using Verenigingstype = ResponseModels.Verenigingstype;
using Werkingsgebied = ResponseModels.Werkingsgebied;

public class PubliekVerenigingDetailAllMapper
{
    private readonly VerenigingstypeMapperV2 _verenigingstypeMapper;

    public PubliekVerenigingDetailAllMapper()
    {
        _verenigingstypeMapper = new VerenigingstypeMapperV2();
    }

    public PubliekVerenigingDetailResponse Map(PubliekVerenigingDetailDocument document, AppSettings appSettings)
        => new()
        {
            Context = $"{appSettings.BaseUrl}/v1/contexten/publiek/detail-all-vereniging-context.json",
            Vereniging = new Vereniging
            {
                type = document.JsonLdMetadataType,
                VCode = document.VCode,
                Verenigingstype = Map(document.Verenigingstype),
                Verenigingssubtype =
                    _verenigingstypeMapper.MapSubtype<Verenigingssubtype, PubliekVerenigingDetailDocument.Types.Verenigingssubtype>(
                        document.Verenigingssubtype),
                SubverenigingVan = _verenigingstypeMapper.MapSubverenigingVan(
                    document.Verenigingssubtype,
                    () => new SubverenigingVan()
                    {
                        AndereVereniging = document.SubverenigingVan.AndereVereniging,
                        Identificatie = document.SubverenigingVan.Identificatie,
                        Beschrijving = document.SubverenigingVan.Beschrijving,
                    }),
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
                Relaties = document.Relaties.Select(r => Map(appSettings, r)).ToArray(),
                Lidmaatschappen = document.Lidmaatschappen.Select(l => Map(l)).ToArray(),
            },
            Metadata = new Metadata { DatumLaatsteAanpassing = document.DatumLaatsteAanpassing },
        };

    private Lidmaatschap Map(PubliekVerenigingDetailDocument.Types.Lidmaatschap l)
        => new()
        {
            id = l.JsonLdMetadata.Id,
            type = l.JsonLdMetadata.Type,
            Beschrijving = l.Beschrijving,
            AndereVereniging = l.AndereVereniging,
            Identificatie = l.Identificatie,
            Van = l.Van.FormatAsBelgianDate(),
            Tot = l.Tot.FormatAsBelgianDate(),
        };

    private Relatie Map(AppSettings appSettings, PubliekVerenigingDetailDocument.Types.Relatie r)
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

    private Contactgegeven Map(PubliekVerenigingDetailDocument.Types.Contactgegeven info)
        => new()
        {
            id = info.JsonLdMetadata.Id,
            type = info.JsonLdMetadata.Type,
            Contactgegeventype = info.Contactgegeventype,
            Waarde = info.Waarde,
            Beschrijving = info.Beschrijving,
            IsPrimair = info.IsPrimair,
        };

    private Verenigingstype Map(PubliekVerenigingDetailDocument.Types.Verenigingstype verenigingstype)
        => new()
        {
            Code = verenigingstype.Code,
            Naam = verenigingstype.Naam,
        };

    private Sleutel Map(PubliekVerenigingDetailDocument.Types.Sleutel s)
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

    private HoofdactiviteitVerenigingsloket Map(PubliekVerenigingDetailDocument.Types.HoofdactiviteitVerenigingsloket ha)
        => new()
        {
            id = ha.JsonLdMetadata.Id,
            type = ha.JsonLdMetadata.Type,
            Code = ha.Code,
            Naam = ha.Naam,
        };

    private Werkingsgebied Map(PubliekVerenigingDetailDocument.Types.Werkingsgebied wg)
        => new()
        {
            id = wg.JsonLdMetadata.Id,
            type = wg.JsonLdMetadata.Type,
            Code = wg.Code,
            Naam = wg.Naam,
        };

    private Locatie Map(PubliekVerenigingDetailDocument.Types.Locatie loc)
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

    private AdresVerwijzing? Map(PubliekVerenigingDetailDocument.Types.Locatie.AdresVerwijzing? verwijzing)
    {
        if (verwijzing is null) return null;

        return new AdresVerwijzing
        {
            id = verwijzing.JsonLdMetadata.Id,
            type = verwijzing.JsonLdMetadata.Type,
        };
    }

    private AdresId? Map(PubliekVerenigingDetailDocument.Types.AdresId? adresId)
        => adresId is not null
            ? new AdresId
            {
                Broncode = adresId.Broncode,
                Bronwaarde = adresId.Bronwaarde,
            }
            : null;

    private Adres? Map(PubliekVerenigingDetailDocument.Types.Adres? adres)
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

public class EmptyStringNamenVoorLidmaatschapMapper : INamenVoorLidmaatschapMapper
{
    public string MapNaamVoorLidmaatschap(string vCode)
        => string.Empty;
}
