namespace AssociationRegistry.Admin.ProjectionHost.Projections.PowerBiExport;

using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging;
using Events;
using Formats;
using Schema;
using Schema.PowerBiExport;
using Contactgegeven = Schema.PowerBiExport.Contactgegeven;
using Doelgroep = Schema.PowerBiExport.Doelgroep;
using HoofdactiviteitVerenigingsloket = Schema.PowerBiExport.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = Schema.PowerBiExport.Lidmaatschap;
using Locatie = Schema.PowerBiExport.Locatie;
using Verenigingstype = Schema.PowerBiExport.Verenigingstype;
using Vertegenwoordiger = Schema.PowerBiExport.Vertegenwoordiger;
using Werkingsgebied = Schema.PowerBiExport.Werkingsgebied;

public class PowerBiExportMapper
{
      public static Lidmaatschap MapLidmaatschap(Registratiedata.Lidmaatschap lid, string vCode) =>
        new(
            lid.LidmaatschapId,
            lid.AndereVereniging,
            lid.DatumVan.FormatAsBelgianDate(),
            lid.DatumTot.FormatAsBelgianDate(),
            lid.Identificatie,
            lid.Beschrijving
        );

    public static Locatie MapLocatie(Registratiedata.Locatie loc, string bron, string vCode) =>
        new()
        {
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
        };
    }

    public static Adres? MapAdres(Registratiedata.AdresUitAdressenregister? adres, string vCode, int locId) =>
        adres is null
            ? null
            : new Adres
            {
                Straatnaam = adres.Straatnaam,
                Huisnummer = adres.Huisnummer,
                Busnummer = adres.Busnummer,
                Postcode = adres.Postcode,
                Gemeente = adres.Gemeente,
                Land = DecentraalBeheer.Vereniging.Adressen.Adres.België,
            };

    public static Adres? MapAdres(Registratiedata.Adres? adres, string vCode, int locId) =>
        adres is null
            ? null
            : new Adres
            {
                Straatnaam = adres.Straatnaam,
                Huisnummer = adres.Huisnummer,
                Busnummer = adres.Busnummer,
                Postcode = adres.Postcode,
                Gemeente = adres.Gemeente,
                Land = adres.Land,
            };

    public static AdresId? MapAdresId(Registratiedata.AdresId? locAdresId) =>
        locAdresId is null ? null : new AdresId { Bronwaarde = locAdresId.Bronwaarde, Broncode = locAdresId.Broncode };

    public static Contactgegeven MapContactgegeven(Registratiedata.Contactgegeven c, string bron, string vCode) =>
        new()
        {
            ContactgegevenId = c.ContactgegevenId,
            Contactgegeventype = c.Contactgegeventype,
            Waarde = c.Waarde,
            Beschrijving = c.Beschrijving,
            IsPrimair = c.IsPrimair,
            Bron = bron,
        };

    public static Vertegenwoordiger MapVertegenwoordiger(
        Registratiedata.Vertegenwoordiger v,
        string bron,
        string vCode
    ) =>
        new()
        {
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
                Email = v.Email,
                Telefoon = v.Telefoon,
                Mobiel = v.Mobiel,
                SocialMedia = v.SocialMedia,
                IsPrimair = v.IsPrimair,
            },
            Bron = bron,
        };

    public static HoofdactiviteitVerenigingsloket MapHoofdactiviteitVerenigingsloket(
        Registratiedata.HoofdactiviteitVerenigingsloket h
    ) =>
        new()
        {
            Code = h.Code,
            Naam = h.Naam,
        };

    public static Werkingsgebied MapWerkingsgebied(Registratiedata.Werkingsgebied w) =>
        new()
        {
            Code = w.Code,
            Naam = w.Naam,
        };

    public static Verenigingstype MapVerenigingstype(DecentraalBeheer.Vereniging.Verenigingstype verenigingstype) =>
        new() { Code = verenigingstype.Code, Naam = verenigingstype.Naam };

    public static Sleutel MapKboSleutel(string kboNummer, string vCode) =>
        new()
        {
            Bron = Sleutelbron.KBO.Waarde,
            Waarde = kboNummer,
            CodeerSysteem = CodeerSysteem.KBO.Waarde,
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                Nummer = kboNummer,
            },
        };

    public static Sleutel MapVrSleutel(string vCode) =>
        new()
        {
            Bron = Sleutelbron.VR.Waarde,
            Waarde = vCode,
            CodeerSysteem = CodeerSysteem.VR.Waarde,
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                Nummer = vCode,
            },
        };

    public static Doelgroep MapDoelgroep(Registratiedata.Doelgroep doelgroep, string vCode) =>
        new()
        {
            Minimumleeftijd = doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroep.Maximumleeftijd,
        };
    public static Bankrekeningnummer MapBankrekeningnummer(
        int bankrekeningnummerId,
        string doel,
        string[] bevestigdDoor,
        string bron
    ) =>
        new(bankrekeningnummerId, doel, bevestigdDoor, bron);

    public static JsonLdMetadata CreateJsonLdMetadata(JsonLdType jsonLdType, params string[] idValues) =>
        new() { Id = jsonLdType.CreateWithIdValues(idValues), Type = jsonLdType.Type };
}
