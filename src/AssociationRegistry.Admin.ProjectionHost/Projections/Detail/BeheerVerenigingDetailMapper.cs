namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Events;
using Formats;
using JsonLdContext;
using Schema;
using Schema.Detail;
using Vereniging;
using Adres = Schema.Detail.Adres;
using AdresId = Schema.Detail.AdresId;
using Contactgegeven = Schema.Detail.Contactgegeven;
using Doelgroep = Schema.Detail.Doelgroep;
using HoofdactiviteitVerenigingsloket = Schema.Detail.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = Schema.Detail.Lidmaatschap;
using Locatie = Schema.Detail.Locatie;
using Vertegenwoordiger = Schema.Detail.Vertegenwoordiger;
using Werkingsgebied = Schema.Detail.Werkingsgebied;

public class BeheerVerenigingDetailMapper
{
    public static Lidmaatschap MapLidmaatschap(Registratiedata.Lidmaatschap lid, string vCode)
        => new(CreateJsonLdMetadata(JsonLdType.Locatie, vCode, lid.LidmaatschapId.ToString()),
               lid.LidmaatschapId,
               lid.AndereVereniging,
               lid.DatumVan,
               lid.DatumTot,
               lid.Identificatie,
               lid.Beschrijving
        );

    public static Locatie MapLocatie(Registratiedata.Locatie loc, string bron, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, vCode, loc.LocatieId.ToString()),
            LocatieId = loc.LocatieId,
            IsPrimair = loc.IsPrimair,
            Naam = loc.Naam,
            VerwijstNaar = MapAdresVerwijzing(loc.AdresId),
            Locatietype = loc.Locatietype,
            Adres = MapAdres(loc.Adres, vCode, loc.LocatieId),
            Adresvoorstelling = loc.Adres.ToAdresString(),
            AdresId = MapAdresId(loc.AdresId),
            Bron = bron,
        };

    public static AdresVerwijzing? MapAdresVerwijzing(Registratiedata.AdresId? adresId)
    {
        if (adresId is null)
            return null;

        return new AdresVerwijzing
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.AdresVerwijzing, adresId.Bronwaarde.Split('/').Last()),
        };
    }

    public static Adres? MapAdres(Registratiedata.AdresUitAdressenregister? adres, string vCode, int locId)
        => adres is null
            ? null
            : new Adres
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Adres, vCode, locId.ToString()),
                Straatnaam = adres.Straatnaam,
                Huisnummer = adres.Huisnummer,
                Busnummer = adres.Busnummer,
                Postcode = adres.Postcode,
                Gemeente = adres.Gemeente,
                Land = AssociationRegistry.Vereniging.Adres.België,
            };

    public static Adres? MapAdres(Registratiedata.Adres? adres, string vCode, int locId)
        => adres is null
            ? null
            : new Adres
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Adres, vCode, locId.ToString()),
                Straatnaam = adres.Straatnaam,
                Huisnummer = adres.Huisnummer,
                Busnummer = adres.Busnummer,
                Postcode = adres.Postcode,
                Gemeente = adres.Gemeente,
                Land = adres.Land,
            };

    public static AdresId? MapAdresId(Registratiedata.AdresId? locAdresId)
        => locAdresId is null
            ? null
            : new AdresId
            {
                Bronwaarde = locAdresId.Bronwaarde,
                Broncode = locAdresId.Broncode,
            };

    public static Contactgegeven MapContactgegeven(Registratiedata.Contactgegeven c, string bron, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Contactgegeven, vCode, c.ContactgegevenId.ToString()),
            ContactgegevenId = c.ContactgegevenId,
            Contactgegeventype = c.Contactgegeventype,
            Waarde = c.Waarde,
            Beschrijving = c.Beschrijving,
            IsPrimair = c.IsPrimair,
            Bron = bron,
        };

    public static Vertegenwoordiger MapVertegenwoordiger(Registratiedata.Vertegenwoordiger v, string bron, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Vertegenwoordiger, vCode, v.VertegenwoordigerId.ToString()),
            VertegenwoordigerId = v.VertegenwoordigerId,
            Insz = v.Insz,
            IsPrimair = v.IsPrimair,
            Roepnaam = v.Roepnaam,
            Rol = v.Rol,
            Achternaam = v.Achternaam,
            Voornaam = v.Voornaam,
            Email = v.Email,
            Telefoon = v.Telefoon,
            Mobiel = v.Mobiel,
            SocialMedia = v.SocialMedia,
            VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.VertegenwoordigerContactgegeven, vCode, v.VertegenwoordigerId.ToString()),
                Email = v.Email,
                Telefoon = v.Telefoon,
                Mobiel = v.Mobiel,
                SocialMedia = v.SocialMedia,
                IsPrimair = v.IsPrimair,
            },
            Bron = bron,
        };

    public static HoofdactiviteitVerenigingsloket MapHoofdactiviteitVerenigingsloket(
        Registratiedata.HoofdactiviteitVerenigingsloket h)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Hoofdactiviteit, h.Code),
            Code = h.Code,
            Naam = h.Naam,
        };

    public static Werkingsgebied MapWerkingsgebied(
        Registratiedata.Werkingsgebied w)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Werkingsgebied, w.Code),
            Code = w.Code,
            Naam = w.Naam,
        };

    public static VerenigingsType MapVerenigingsType(Verenigingstype verenigingstype)
        => new()
        {
            Code = verenigingstype.Code,
            Naam = verenigingstype.Naam,
        };

    public static Sleutel MapKboSleutel(string kboNummer, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, vCode, Sleutelbron.KBO.Waarde),
            Bron = Sleutelbron.KBO.Waarde,
            Waarde = kboNummer,
            CodeerSysteem = CodeerSysteem.KBO.Waarde,
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, vCode, Sleutelbron.KBO.Waarde), Nummer = kboNummer,
            },
        };

    public static Sleutel MapVrSleutel(string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, vCode, Sleutelbron.VR.Waarde),
            Bron = Sleutelbron.VR.Waarde,
            Waarde = vCode,
            CodeerSysteem = CodeerSysteem.VR.Waarde,
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, vCode, Sleutelbron.VR.Waarde),
                Nummer = vCode,
            },
        };

    public static Doelgroep MapDoelgroep(Registratiedata.Doelgroep doelgroep, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Doelgroep, vCode),
            Minimumleeftijd = doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroep.Maximumleeftijd,
        };

    public static JsonLdMetadata CreateJsonLdMetadata(JsonLdType jsonLdType, params string[] idValues)
        => new()
        {
            Id = jsonLdType.CreateWithIdValues(idValues),
            Type = jsonLdType.Type,
        };
}
