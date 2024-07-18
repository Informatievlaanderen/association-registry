namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using Hosts.Configuration.ConfigurationBindings;
using ResponseModels;
using Schema.Detail;
using System.Linq;
using Adres = ResponseModels.Adres;
using AdresId = ResponseModels.AdresId;
using AdresVerwijzing = ResponseModels.AdresVerwijzing;
using Contactgegeven = ResponseModels.Contactgegeven;
using GestructureerdeIdentificator = ResponseModels.GestructureerdeIdentificator;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = ResponseModels.Locatie;
using Relatie = ResponseModels.Relatie;
using Sleutel = ResponseModels.Sleutel;
using VerenigingsType = ResponseModels.VerenigingsType;
using Vertegenwoordiger = ResponseModels.Vertegenwoordiger;
using VertegenwoordigerContactgegevens = ResponseModels.VertegenwoordigerContactgegevens;

public class BeheerVerenigingDetailMapper
{
    private readonly AppSettings _appSettings;

    public BeheerVerenigingDetailMapper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public DetailVerenigingResponse Map(BeheerVerenigingDetailDocument vereniging)
        => new()
        {
            Context = $"{_appSettings.PublicApiBaseUrl}/v1/contexten/beheer/detail-vereniging-context.json",
            Vereniging = Map(vereniging, _appSettings.BaseUrl),
            Metadata = MapMetadata(vereniging),
        };

    private static Metadata MapMetadata(BeheerVerenigingDetailDocument vereniging)
        => new()
        {
            DatumLaatsteAanpassing = vereniging.DatumLaatsteAanpassing,
        };

    private static VerenigingDetail Map(BeheerVerenigingDetailDocument vereniging, string baseUrl)
    {
        return new VerenigingDetail
        {
            type = vereniging.JsonLdMetadataType,
            VCode = vereniging.VCode,
            Verenigingstype = Map(vereniging.Verenigingstype),
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
            Sleutels = vereniging.Sleutels.Select(Map).ToArray(),
            Relaties = vereniging.Relaties.Select(relatie => Map(relatie, baseUrl)).ToArray(),
            Bron = vereniging.Bron,
        };
    }

    private static Relatie Map(Schema.Detail.Relatie relatie, string baseUrl)
        => new()
        {
            Relatietype = relatie.Relatietype,
            AndereVereniging = Map(relatie.AndereVereniging, baseUrl),
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

    private static VerenigingsType Map(Schema.Detail.VerenigingsType verenigingsType)
        => new()
        {
            Code = verenigingsType.Code,
            Naam = verenigingsType.Naam,
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
