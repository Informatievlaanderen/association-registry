namespace AssociationRegistry.Test.E2E.Framework.Mappers;

using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Detail.ResponseModels;
using Common.Framework;
using Events;
using JsonLdContext;
using Vereniging;
using Vereniging.Bronnen;
using Contactgegeven = Admin.Api.Verenigingen.Detail.ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = Admin.Api.Verenigingen.Detail.ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = Admin.Api.Verenigingen.Detail.ResponseModels.Locatie;
using Vertegenwoordiger = Admin.Api.Verenigingen.Detail.ResponseModels.Vertegenwoordiger;
using Werkingsgebied = Admin.Api.Verenigingen.Detail.ResponseModels.Werkingsgebied;

public class BeheerDetailResponseMapper
{

    public static Sleutel[] MapSleutels(string vCode)
        =>
        [
            new()
            {
                Bron = Sleutelbron.VR.Waarde,
                id = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                type = JsonLdType.Sleutel.Type,
                Waarde = vCode,
                CodeerSysteem = CodeerSysteem.VR,
                GestructureerdeIdentificator = new GestructureerdeIdentificator
                {
                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                    type = JsonLdType.GestructureerdeSleutel.Type,
                    Nummer = vCode,
                },
            },
        ];

    public static Sleutel[] MapSleutels(string vCode, string kboNummer)
        =>
        [
            new()
            {
                @id = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                @type = JsonLdType.Sleutel.Type,
                Bron = Sleutelbron.VR.Waarde,
                GestructureerdeIdentificator = new()
                {
                    @id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                    @type = JsonLdType.GestructureerdeSleutel.Type,
                    Nummer = vCode,
                },
                Waarde = vCode,
                CodeerSysteem = CodeerSysteem.VR,
            },
            new()
            {
                @id = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.KBO.Waarde),
                @type = JsonLdType.Sleutel.Type,
                Bron = Sleutelbron.KBO.Waarde,
                GestructureerdeIdentificator = new()
                {
                    @id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.KBO.Waarde),
                    @type = JsonLdType.GestructureerdeSleutel.Type,
                    Nummer = kboNummer,
                },
                Waarde = kboNummer,
                CodeerSysteem = CodeerSysteem.KBO,
            },
        ];

    public static Vertegenwoordiger[] MapVertegenwoordigers(ToeTeVoegenVertegenwoordiger[] vertegenwoordigers, string vCode)
    {
        return vertegenwoordigers.Select((x, i) => new Vertegenwoordiger
        {
            id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Vertegenwoordiger.Type,
            VertegenwoordigerId = i + 1,
            PrimairContactpersoon = x.IsPrimair,
            Achternaam = x.Achternaam,
            Email = x.Email,
            Insz = x.Insz,
            Voornaam = x.Voornaam,
            Roepnaam = x.Roepnaam,
            Rol = x.Rol,
            Telefoon = x.Telefoon,
            Mobiel = x.Mobiel,
            SocialMedia = x.SocialMedia,
            VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
            {
                id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(
                    vCode, $"{i + 1}"),
                type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                IsPrimair = x.IsPrimair,
                Email = x.Email,
                Telefoon = x.Telefoon,
                Mobiel = x.Mobiel,
                SocialMedia = x.SocialMedia,
            },
            Bron = Bron.Initiator,
        }).ToArray();
    }

    public static Vertegenwoordiger[] MapVertegenwoordigers(Registratiedata.Vertegenwoordiger[] vertegenwoordigers, string vCode)
    {
        return vertegenwoordigers.Select((x, i) => new Vertegenwoordiger
        {
            id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Vertegenwoordiger.Type,
            VertegenwoordigerId = i + 1,
            PrimairContactpersoon = x.IsPrimair,
            Achternaam = x.Achternaam,
            Email = x.Email,
            Insz = x.Insz,
            Voornaam = x.Voornaam,
            Roepnaam = x.Roepnaam,
            Rol = x.Rol,
            Telefoon = x.Telefoon,
            Mobiel = x.Mobiel,
            SocialMedia = x.SocialMedia,
            VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
            {
                id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(
                    vCode, $"{i + 1}"),
                type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                IsPrimair = x.IsPrimair,
                Email = x.Email,
                Telefoon = x.Telefoon,
                Mobiel = x.Mobiel,
                SocialMedia = x.SocialMedia,
            },
            Bron = Bron.Initiator,
        }).ToArray();
    }

    public static Contactgegeven[] MapContactgegevens(Registratiedata.Contactgegeven[] toeTeVoegenContactgegevens, string vCode)
    {
        return toeTeVoegenContactgegevens.Select((x, i) => new Contactgegeven
        {
            id = JsonLdType.Contactgegeven.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Contactgegeven.Type,
            ContactgegevenId = i + 1,
            Contactgegeventype = x.Contactgegeventype,
            Waarde = x.Waarde,
            Beschrijving = x.Beschrijving!,
            IsPrimair = x.IsPrimair,
            Bron = Bron.Initiator,
        }).ToArray();
    }

    public static Contactgegeven[] MapContactgegevens(ToeTeVoegenContactgegeven[] toeTeVoegenContactgegevens, string vCode)
    {
        return toeTeVoegenContactgegevens.Select((x, i) => new Contactgegeven
        {
            id = JsonLdType.Contactgegeven.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Contactgegeven.Type,
            ContactgegevenId = i + 1,
            Contactgegeventype = x.Contactgegeventype,
            Waarde = x.Waarde,
            Beschrijving = x.Beschrijving!,
            IsPrimair = x.IsPrimair,
            Bron = Bron.Initiator,
        }).ToArray();
    }

    public static Locatie[] MapLocaties(ToeTeVoegenLocatie[] toeTeVoegenLocaties, string vCode)
    {
        return toeTeVoegenLocaties.Select((x, i) => new Locatie
        {
            id = JsonLdType.Locatie.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Locatie.Type,
            LocatieId = i + 1,
            Locatietype = x.Locatietype,
            Naam = x.Naam,
            Bron = Bron.Initiator,
            IsPrimair = x.IsPrimair,
        }).ToArray();
    }

    public static Locatie[] MapLocaties(Registratiedata.Locatie[] toeTeVoegenLocaties, string vCode)
    {
        return toeTeVoegenLocaties.Select((x, i) => new Locatie
        {
            id = JsonLdType.Locatie.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Locatie.Type,
            LocatieId = i + 1,
            Locatietype = x.Locatietype,
            Naam = x.Naam,
            Bron = Bron.Initiator,
            IsPrimair = x.IsPrimair,
        }).ToArray();
    }

    public static HoofdactiviteitVerenigingsloket[] MapHoofdactiviteitenVerenigingsloket(
        string[] hoofdactiviteitenVerenigingsloket)
    {
        return hoofdactiviteitenVerenigingsloket.Select(x =>
        {
            var hoofdactiviteitVerenigingsloket = AssociationRegistry.Vereniging.HoofdactiviteitVerenigingsloket.Create(x);

            return new HoofdactiviteitVerenigingsloket
            {
                Code = hoofdactiviteitVerenigingsloket.Code,
                Naam = hoofdactiviteitVerenigingsloket.Naam,
                id = JsonLdType.Hoofdactiviteit.CreateWithIdValues(hoofdactiviteitVerenigingsloket.Code),
                type = JsonLdType.Hoofdactiviteit.Type,
            };
        }).ToArray();
    }

    public static Werkingsgebied[] MapWerkingsgebieden(string[] werkingsgebieden)
    {
        var werkingsgebiedenServiceMock = new WerkingsgebiedenServiceMock();
        return werkingsgebieden.Select(x =>
        {
            var werkingsgebied = werkingsgebiedenServiceMock.Create(x);

            return new Werkingsgebied
            {
                Code = werkingsgebied.Code,
                Naam = werkingsgebied.Naam,
                id = JsonLdType.Werkingsgebied.CreateWithIdValues(werkingsgebied.Code),
                type = JsonLdType.Werkingsgebied.Type,
            };
        }).ToArray();
    }
}
