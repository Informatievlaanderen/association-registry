namespace AssociationRegistry.Test.E2E.Framework.Mappers;

using Admin.Api.Verenigingen.Search.ResponseModels;
using Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Events;
using JsonLdContext;
using Vereniging;
using HoofdactiviteitVerenigingsloket = Admin.Api.Verenigingen.Search.ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = Admin.Api.Verenigingen.Search.ResponseModels.Locatie;
using Werkingsgebied = Admin.Api.Verenigingen.Search.ResponseModels.Werkingsgebied;

public class BeheerZoekResponseMapper
{
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

    public static Sleutel[] MapSleutels(WijzigBasisgegevensRequest request, string vCode)
        =>
        [
            new Sleutel
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

    public static Locatie[] MapLocaties(Registratiedata.Locatie[] locaties, string vCode)
    {
        return locaties.Select((x, i) => new Locatie
        {
            id = JsonLdType.Locatie.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Locatie.Type,
            Locatietype = x.Locatietype,
            Naam = x.Naam,
            IsPrimair = x.IsPrimair,
        }).ToArray();
    }

    public static Werkingsgebied[] MapWerkingsgebieden(
        string[] werkingsgebieden)
    {
        return werkingsgebieden.Select(x =>
        {
            var werkingsgebied = AssociationRegistry.Vereniging.Werkingsgebied.Create(x);

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
