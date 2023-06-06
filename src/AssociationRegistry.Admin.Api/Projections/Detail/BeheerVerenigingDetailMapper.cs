namespace AssociationRegistry.Admin.Api.Projections.Detail;

using Events;
using Infrastructure.Extensions;
using Vereniging;

public class BeheerVerenigingDetailMapper
{
    public static BeheerVerenigingDetailDocument.Locatie MapLocatie(Registratiedata.Locatie loc)
        => new()
        {
            Hoofdlocatie = loc.Hoofdlocatie,
            Naam = loc.Naam,
            Locatietype = loc.Locatietype,
            Straatnaam = loc.Straatnaam,
            Huisnummer = loc.Huisnummer,
            Busnummer = loc.Busnummer,
            Postcode = loc.Postcode,
            Gemeente = loc.Gemeente,
            Land = loc.Land,
            Adres = loc.ToAdresString(),
        };

    public static BeheerVerenigingDetailDocument.Contactgegeven MapContactgegeven(Registratiedata.Contactgegeven c)
        => new()
        {
            ContactgegevenId = c.ContactgegevenId,
            Type = c.Type,
            Waarde = c.Waarde,
            Beschrijving = c.Beschrijving,
            IsPrimair = c.IsPrimair,
        };

    public static BeheerVerenigingDetailDocument.Vertegenwoordiger MapVertegenwoordiger(Registratiedata.Vertegenwoordiger v)
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
                ExternId = moederverenigingsData.KboNummer,
                VCode = moederverenigingsData.VCode,
                Naam = moederverenigingsData.Naam,
            },
        };

    public static BeheerVerenigingDetailDocument.Sleutel MapKboSleutel(string kboNummer)
        => new()
        {
            Bron = Bron.Kbo.Waarde,
            Waarde = kboNummer,
        };
}
