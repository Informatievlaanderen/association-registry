namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Events;
using Formatters;
using Schema.Detail;
using Vereniging;
using Adres = Schema.Detail.Adres;
using AdresId = Schema.Detail.AdresId;
using Doelgroep = Schema.Detail.Doelgroep;

public class BeheerVerenigingDetailMapper
{
    public static BeheerVerenigingDetailDocument.Locatie MapLocatie(Registratiedata.Locatie loc, string initiator)
        => new()
        {
            LocatieId = loc.LocatieId,
            IsPrimair = loc.IsPrimair,
            Naam = loc.Naam,
            Locatietype = loc.Locatietype,
            Adres = Map(loc.Adres),
            Adresvoorstelling = loc.Adres.ToAdresString(),
            AdresId = Map(loc.AdresId),
            Bron = initiator,
        };

    private static Adres? Map(Registratiedata.Adres? adres)
        => adres is null
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

    private static AdresId? Map(Registratiedata.AdresId? locAdresId)
        => locAdresId is null
            ? null
            : new AdresId
            {
                Bronwaarde = locAdresId.Bronwaarde,
                Broncode = locAdresId.Broncode,
            };

    public static BeheerVerenigingDetailDocument.Contactgegeven MapContactgegeven(Registratiedata.Contactgegeven c, string initiator)
        => new()
        {
            ContactgegevenId = c.ContactgegevenId,
            Type = c.Type,
            Waarde = c.Waarde,
            Beschrijving = c.Beschrijving,
            IsPrimair = c.IsPrimair,
            Bron = initiator,
        };

    public static BeheerVerenigingDetailDocument.Vertegenwoordiger MapVertegenwoordiger(Registratiedata.Vertegenwoordiger v, string initiator)
        => new()
        {
            VertegenwoordigerId = v.VertegenwoordigerId,
            IsPrimair = v.IsPrimair,
            Roepnaam = v.Roepnaam,
            Rol = v.Rol,
            Achternaam = v.Achternaam,
            Voornaam = v.Voornaam,
            Email = v.Email,
            Telefoon = v.Telefoon,
            Mobiel = v.Mobiel,
            SocialMedia = v.SocialMedia,
            Bron = initiator,
        };

    public static BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket MapHoofdactiviteitVerenigingsloket(Registratiedata.HoofdactiviteitVerenigingsloket h)
        => new()
        {
            Code = h.Code,
            Beschrijving = h.Beschrijving,
        };

    public static BeheerVerenigingDetailDocument.VerenigingsType MapVerenigingsType(Verenigingstype verenigingstype)
        => new()
        {
            Code = verenigingstype.Code,
            Beschrijving = verenigingstype.Beschrijving,
        };

    public static BeheerVerenigingDetailDocument.Relatie MapMoederRelatie(AfdelingWerdGeregistreerd.MoederverenigingsData moederverenigingsData)
        => new()
        {
            Type = RelatieType.IsAfdelingVan.Beschrijving,
            AndereVereniging = new BeheerVerenigingDetailDocument.Relatie.GerelateerdeVereniging
            {
                KboNummer = moederverenigingsData.KboNummer,
                VCode = moederverenigingsData.VCode,
                Naam = moederverenigingsData.Naam,
            },
        };

    public static BeheerVerenigingDetailDocument.Sleutel MapKboSleutel(string kboNummer)
        => new()
        {
            Bron = Verenigingsbron.Kbo.Waarde,
            Waarde = kboNummer,
        };

    public static Doelgroep MapDoelgroep(Registratiedata.Doelgroep doelgroep)
        => new()
        {
            Minimumleeftijd = doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroep.Maximumleeftijd,
        };
}
