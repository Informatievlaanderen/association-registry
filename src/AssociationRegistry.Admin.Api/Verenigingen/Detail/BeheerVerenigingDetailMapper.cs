namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Schema.Detail;
using System.Linq;
using Adres = ResponseModels.Adres;
using AdresId = ResponseModels.AdresId;

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
            VCode = vereniging.VCode,
            Type = Map(vereniging.Type),
            Naam = vereniging.Naam,
            Roepnaam = vereniging.Roepnaam,
            KorteNaam = vereniging.KorteNaam,
            KorteBeschrijving = vereniging.KorteBeschrijving,
            Startdatum = vereniging.Startdatum,
            Einddatum = vereniging.Einddatum,
            Doelgroep = new DoelgroepResponse
            {
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

    private static Relatie Map(BeheerVerenigingDetailDocument.Relatie relatie, string baseUrl)
        => new()
        {
            Type = relatie.Type,
            AndereVereniging = Map(relatie.AndereVereniging, baseUrl),
        };

    private static GerelateerdeVereniging Map(
        BeheerVerenigingDetailDocument.Relatie.GerelateerdeVereniging gerelateerdeVereniging,
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

    private static VerenigingsType Map(BeheerVerenigingDetailDocument.VerenigingsType verenigingsType)
        => new()
        {
            Code = verenigingsType.Code,
            Beschrijving = verenigingsType.Beschrijving,
        };

    private static Sleutel Map(BeheerVerenigingDetailDocument.Sleutel sleutel)
        => new()
        {
            Bron = sleutel.Bron,
            Waarde = sleutel.Waarde,
        };

    private static Contactgegeven Map(BeheerVerenigingDetailDocument.Contactgegeven contactgegeven)
        => new()
        {
            ContactgegevenId = contactgegeven.ContactgegevenId,
            Type = contactgegeven.Type,
            Waarde = contactgegeven.Waarde,
            Beschrijving = contactgegeven.Beschrijving,
            IsPrimair = contactgegeven.IsPrimair,
            Bron = contactgegeven.Bron,
        };

    private static HoofdactiviteitVerenigingsloket Map(
        BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket hoofdactiviteitVerenigingsloket)
        => new()
        {
            Code = hoofdactiviteitVerenigingsloket.Code,
            Naam = hoofdactiviteitVerenigingsloket.Naam,
        };

    private static Vertegenwoordiger Map(BeheerVerenigingDetailDocument.Vertegenwoordiger ver)
        => new()
        {
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
            Bron = ver.Bron,
        };

    private static Locatie Map(BeheerVerenigingDetailDocument.Locatie loc)
        => new()
        {
            LocatieId = loc.LocatieId,
            Locatietype = loc.Locatietype,
            IsPrimair = loc.IsPrimair,
            Adresvoorstelling = loc.Adresvoorstelling,
            Naam = loc.Naam,
            Adres = Map(loc.Adres),
            AdresId = Map(loc.AdresId),
            Bron = loc.Bron,
        };

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
                Straatnaam = adres.Straatnaam,
                Huisnummer = adres.Huisnummer,
                Busnummer = adres.Busnummer,
                Postcode = adres.Postcode,
                Gemeente = adres.Gemeente,
                Land = adres.Land,
            }
            : null;
}
