namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using AssociationRegistry.Admin.Schema.Detail;
using Formats;
using Hosts.Configuration.ConfigurationBindings;
using Infrastructure;
using ResponseModels;
using Vereniging;
using Vereniging.Mappers;
using Adres = ResponseModels.Adres;
using AdresId = ResponseModels.AdresId;
using AdresVerwijzing = ResponseModels.AdresVerwijzing;
using Contactgegeven = ResponseModels.Contactgegeven;
using GestructureerdeIdentificator = ResponseModels.GestructureerdeIdentificator;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = ResponseModels.Lidmaatschap;
using Locatie = ResponseModels.Locatie;
using Relatie = ResponseModels.Relatie;
using Sleutel = ResponseModels.Sleutel;
using SubverenigingVan = ResponseModels.SubverenigingVan;
using Verenigingssubtype = ResponseModels.Verenigingssubtype;
using Verenigingstype = ResponseModels.Verenigingstype;
using Vertegenwoordiger = ResponseModels.Vertegenwoordiger;
using VertegenwoordigerContactgegevens = ResponseModels.VertegenwoordigerContactgegevens;
using Werkingsgebied = ResponseModels.Werkingsgebied;

public class BeheerVerenigingDetailMapper
{
    private readonly AppSettings _appSettings;
    private readonly IVerplichteNamenVoorVCodesMapper _verplichteNamenVoorVCodesMapper;
    private readonly string? _version;
    private readonly IVerenigingstypeMapper _verenigingstypeMapper;

    public BeheerVerenigingDetailMapper(
        AppSettings appSettings,
        IVerplichteNamenVoorVCodesMapper verplichteNamenVoorVCodesMapper,
        string? version)
    {
        _appSettings = appSettings;
        _verplichteNamenVoorVCodesMapper = verplichteNamenVoorVCodesMapper;
        _verenigingstypeMapper = version == WellknownVersions.V2 ? new VerenigingstypeMapperV2() : new VerenigingstypeMapperV1();
        _version = version;
    }

    public DetailVerenigingResponse Map(BeheerVerenigingDetailDocument vereniging)
        => new()
        {
            Context = $"{_appSettings.PublicApiBaseUrl}/v1/contexten/beheer/detail-vereniging-context.json",
            Vereniging = Map(vereniging, _verplichteNamenVoorVCodesMapper, _appSettings.BaseUrl),
            Metadata = MapMetadata(vereniging),
        };

    private static Metadata MapMetadata(BeheerVerenigingDetailDocument vereniging)
        => new()
        {
            DatumLaatsteAanpassing = vereniging.DatumLaatsteAanpassing,
        };

    private VerenigingDetail Map(
        BeheerVerenigingDetailDocument vereniging,
        IVerplichteNamenVoorVCodesMapper verplichteNamenVoorVCodesMapper,
        string baseUrl)
    {
        return new VerenigingDetail
        {
            type = vereniging.JsonLdMetadataType,
            VCode = vereniging.VCode,
            CorresponderendeVCodes = vereniging.CorresponderendeVCodes,
            Verenigingstype = _verenigingstypeMapper.Map<Verenigingstype, Schema.Detail.Verenigingstype>(vereniging.Verenigingstype),
            Verenigingssubtype =
                _verenigingstypeMapper.MapSubtype<Verenigingssubtype, IVerenigingssubtypeCode>(vereniging.Verenigingssubtype),
            SubverenigingVan =
                _verenigingstypeMapper.MapSubverenigingVan(
                    vereniging.Verenigingssubtype,
                    () => new SubverenigingVan
                    {
                        AndereVereniging = vereniging.SubverenigingVan.AndereVereniging,
                        Naam = _verplichteNamenVoorVCodesMapper.MapNaamVoorVCode(vereniging.SubverenigingVan.AndereVereniging),
                        Identificatie = vereniging.SubverenigingVan.Identificatie,
                        Beschrijving = vereniging.SubverenigingVan.Beschrijving,
                    }),
            Naam = vereniging.Naam,
            Roepnaam = vereniging.Roepnaam,
            KorteNaam = vereniging.KorteNaam,
            KorteBeschrijving = vereniging.KorteBeschrijving,
            Startdatum = vereniging.Startdatum,
            Einddatum = vereniging.Einddatum,
            Doelgroep = new DoelgroepResponse
            {
                id = vereniging.Doelgroep.JsonLdMetadata.Id,
                type = vereniging.Doelgroep.JsonLdMetadata.Type,
                Minimumleeftijd = vereniging.Doelgroep.Minimumleeftijd,
                Maximumleeftijd = vereniging.Doelgroep.Maximumleeftijd,
            },
            Status = vereniging.Status,
            IsUitgeschrevenUitPubliekeDatastroom = vereniging.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens = vereniging.Contactgegevens.Select(Map).ToArray(),
            Locaties = vereniging.Locaties.Select(Map).ToArray(),
            Vertegenwoordigers = vereniging.Vertegenwoordigers.Select(Map).ToArray(),
            HoofdactiviteitenVerenigingsloket = vereniging.HoofdactiviteitenVerenigingsloket.Select(Map).ToArray(),
            Werkingsgebieden = vereniging.Werkingsgebieden.Select(Map).ToArray(),
            Sleutels = vereniging.Sleutels.Select(Map).ToArray(),
            Relaties = vereniging.Relaties.Select(relatie => Map(relatie, baseUrl)).ToArray(),
            Lidmaatschappen = vereniging.Lidmaatschappen.Select(lidmaatschap => Map(lidmaatschap, verplichteNamenVoorVCodesMapper))
                                        .ToArray(),
            Bron = vereniging.Bron,
            IsDubbelVan = vereniging.IsDubbelVan,
        };
    }

    private static Relatie Map(Schema.Detail.Relatie relatie, string baseUrl)
        => new()
        {
            Relatietype = relatie.Relatietype,
            AndereVereniging = Map(relatie.AndereVereniging, baseUrl),
        };

    private static Lidmaatschap Map(
        Schema.Detail.Lidmaatschap lidmaatschap,
        IVerplichteNamenVoorVCodesMapper verplichteNamenVoorVCodesMapper
    )
        => new()
        {
            id = lidmaatschap.JsonLdMetadata.Id,
            type = lidmaatschap.JsonLdMetadata.Type,
            AndereVereniging = lidmaatschap.AndereVereniging,
            Beschrijving = lidmaatschap.Beschrijving,
            Identificatie = lidmaatschap.Identificatie,
            Naam = verplichteNamenVoorVCodesMapper.MapNaamVoorVCode(lidmaatschap.AndereVereniging),
            Van = lidmaatschap.Van.FormatAsBelgianDate(),
            Tot = lidmaatschap.Tot.FormatAsBelgianDate(),
            LidmaatschapId = lidmaatschap.LidmaatschapId,
        };

    private static GerelateerdeVereniging Map(
        Schema.Detail.Relatie.GerelateerdeVereniging gerelateerdeVereniging,
        string baseUrl)
        => new()
        {
            KboNummer = gerelateerdeVereniging.KboNummer,
            VCode = gerelateerdeVereniging.VCode,
            Naam = gerelateerdeVereniging.Naam,
            Detail = !string.IsNullOrEmpty(gerelateerdeVereniging.VCode)
                ? $"{baseUrl}/v1/verenigingen/{gerelateerdeVereniging.VCode}"
                : string.Empty,
        };

    private static Sleutel Map(Schema.Detail.Sleutel sleutel)
        => new()
        {
            id = sleutel.JsonLdMetadata.Id,
            type = sleutel.JsonLdMetadata.Type,
            Bron = sleutel.Bron,
            Waarde = sleutel.Waarde,
            CodeerSysteem = sleutel.CodeerSysteem,
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                id = sleutel.GestructureerdeIdentificator.JsonLdMetadata.Id,
                type = sleutel.GestructureerdeIdentificator.JsonLdMetadata.Type,
                Nummer = sleutel.GestructureerdeIdentificator.Nummer,
            },
        };

    private static Contactgegeven Map(Schema.Detail.Contactgegeven contactgegeven)
        => new()
        {
            id = contactgegeven.JsonLdMetadata.Id,
            type = contactgegeven.JsonLdMetadata.Type,
            ContactgegevenId = contactgegeven.ContactgegevenId,
            Contactgegeventype = contactgegeven.Contactgegeventype,
            Waarde = contactgegeven.Waarde,
            Beschrijving = contactgegeven.Beschrijving,
            IsPrimair = contactgegeven.IsPrimair,
            Bron = contactgegeven.Bron,
        };

    private static HoofdactiviteitVerenigingsloket Map(
        Schema.Detail.HoofdactiviteitVerenigingsloket hoofdactiviteitVerenigingsloket)
        => new()
        {
            id = hoofdactiviteitVerenigingsloket.JsonLdMetadata.Id,
            type = hoofdactiviteitVerenigingsloket.JsonLdMetadata.Type,
            Code = hoofdactiviteitVerenigingsloket.Code,
            Naam = hoofdactiviteitVerenigingsloket.Naam,
        };

    private static Werkingsgebied Map(
        Schema.Detail.Werkingsgebied werkingsgebied)
        => new()
        {
            id = werkingsgebied.JsonLdMetadata.Id,
            type = werkingsgebied.JsonLdMetadata.Type,
            Code = werkingsgebied.Code,
            Naam = werkingsgebied.Naam,
        };

    private static Vertegenwoordiger Map(Schema.Detail.Vertegenwoordiger ver)
        => new()
        {
            id = ver.JsonLdMetadata.Id,
            type = ver.JsonLdMetadata.Type,
            VertegenwoordigerId = ver.VertegenwoordigerId,
            Insz = ver.Insz,
            Voornaam = ver.Voornaam,
            Achternaam = ver.Achternaam,
            Roepnaam = ver.Roepnaam,
            Rol = ver.Rol,
            PrimairContactpersoon = ver.IsPrimair,
            Email = ver.Email,
            Telefoon = ver.Telefoon,
            Mobiel = ver.Mobiel,
            SocialMedia = ver.SocialMedia,
            VertegenwoordigerContactgegevens = Map(ver.VertegenwoordigerContactgegevens),
            Bron = ver.Bron,
        };

    private static VertegenwoordigerContactgegevens Map(Schema.Detail.VertegenwoordigerContactgegevens vc)
        => new()
        {
            id = vc.JsonLdMetadata.Id,
            type = vc.JsonLdMetadata.Type,
            IsPrimair = vc.IsPrimair,
            Email = vc.Email,
            Telefoon = vc.Telefoon,
            Mobiel = vc.Mobiel,
            SocialMedia = vc.SocialMedia,
        };

    private static Locatie Map(Schema.Detail.Locatie loc)
        => new()
        {
            id = loc.JsonLdMetadata.Id,
            type = loc.JsonLdMetadata.Type,
            LocatieId = loc.LocatieId,
            VerwijstNaar = Map(loc.VerwijstNaar),
            Locatietype = loc.Locatietype,
            IsPrimair = loc.IsPrimair,
            Adresvoorstelling = loc.Adresvoorstelling,
            Naam = loc.Naam,
            Adres = Map(loc.Adres),
            AdresId = Map(loc.AdresId),
            Bron = loc.Bron,
        };

    private static AdresVerwijzing? Map(Schema.Detail.AdresVerwijzing? verwijstNaar)
    {
        if (verwijstNaar is null) return null;

        return new AdresVerwijzing
        {
            id = verwijstNaar.JsonLdMetadata.Id,
            type = verwijstNaar.JsonLdMetadata.Type,
        };
    }

    private static AdresId? Map(Schema.Detail.AdresId? adresId)
        => adresId is not null
            ? new AdresId
            {
                Broncode = adresId.Broncode,
                Bronwaarde = adresId.Bronwaarde,
            }
            : null;

    private static Adres? Map(Schema.Detail.Adres? adres)
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

public interface IVerplichteNamenVoorVCodesMapper
{
    string MapNaamVoorVCode(string vCode);
}

public class VerplichteVerplichteNamenVoorVCodesMapper : IVerplichteNamenVoorVCodesMapper
{
    private readonly Dictionary<string, string> _namenVoorLidmaatschap;

    public VerplichteVerplichteNamenVoorVCodesMapper(Dictionary<string, string> namenVoorLidmaatschap)
    {
        _namenVoorLidmaatschap = namenVoorLidmaatschap;
    }

    public string MapNaamVoorVCode(string vCode)
        => _namenVoorLidmaatschap[vCode];
}
