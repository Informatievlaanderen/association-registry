namespace AssociationRegistry.Test.E2E.Framework.Mappers;

using Admin.Api.WebApi.Verenigingen.Common;
using Admin.Api.WebApi.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using Admin.Api.WebApi.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using Formats;
using JsonLdContext;
using Vereniging;
using HoofdactiviteitVerenigingsloket = Admin.Api.WebApi.Verenigingen.Search.ResponseModels.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = Admin.Api.WebApi.Verenigingen.Search.ResponseModels.Lidmaatschap;
using Locatie = Admin.Api.WebApi.Verenigingen.Search.ResponseModels.Locatie;
using Werkingsgebied = Admin.Api.WebApi.Verenigingen.Search.ResponseModels.Werkingsgebied;

public class BeheerZoekResponseMapper
{
    public static HoofdactiviteitVerenigingsloket[] MapHoofdactiviteitenVerenigingsloket(
        string[] hoofdactiviteitenVerenigingsloket)
    {
        return hoofdactiviteitenVerenigingsloket.Select(x =>
        {
            var hoofdactiviteitVerenigingsloket = DecentraalBeheer.Vereniging.HoofdactiviteitVerenigingsloket.Create(x);

            return new HoofdactiviteitVerenigingsloket
            {
                Code = hoofdactiviteitVerenigingsloket.Code,
                Naam = hoofdactiviteitVerenigingsloket.Naam,
                id = JsonLdType.Hoofdactiviteit.CreateWithIdValues(hoofdactiviteitVerenigingsloket.Code),
                type = JsonLdType.Hoofdactiviteit.Type,
            };
        }).ToArray();
    }

    public static HoofdactiviteitVerenigingsloket[] MapHoofdactiviteitenVerenigingsloket(
        Registratiedata.HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket)
    => MapHoofdactiviteitenVerenigingsloket(hoofdactiviteitenVerenigingsloket.Select(x => x.Code).ToArray());

    public static Sleutel[] MapSleutels(string vCode)
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

    public static Locatie[] MapLocaties(ToeTeVoegenLocatie[] toeTeVoegenLocaties, string vCode)
    {
        return toeTeVoegenLocaties.Select((x, i) => new Locatie
        {
            id = JsonLdType.Locatie.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Locatie.Type,
            Locatietype = x.Locatietype,
            Naam = x.Naam,
            IsPrimair = x.IsPrimair,
        }).ToArray();
    }

    public static Locatie[] MapLocaties(Registratiedata.Locatie[] toeTeVoegenLocaties, string vCode)
    {
        return toeTeVoegenLocaties.Select(x => new Locatie
        {
            id = JsonLdType.Locatie.CreateWithIdValues(
                vCode, x.LocatieId.ToString()),
            type = JsonLdType.Locatie.Type,
            Locatietype = x.Locatietype,
            Naam = x.Naam,
            IsPrimair = x.IsPrimair,
        }).ToArray();
    }

    public static Werkingsgebied[] MapWerkingsgebieden(
        string[] werkingsgebieden)
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

    public static Werkingsgebied[] MapWerkingsgebieden(
        Registratiedata.Werkingsgebied[]? werkingsgebieden)
    => MapWerkingsgebieden(werkingsgebieden?.Select(x => x.Code).ToArray() ?? []);

    public static Lidmaatschap[] MapLidmaatschappen(VoegLidmaatschapToeRequest request, string vCode)
        =>
        [
            new Lidmaatschap()
            {
                id = JsonLdType.Lidmaatschap.CreateWithIdValues(
                    vCode, "1"),
                type = JsonLdType.Lidmaatschap.Type,
                AndereVereniging = request.AndereVereniging,
                Beschrijving = request.Beschrijving,
                Identificatie = request.Identificatie,
                Van = request.Van!.Value.FormatAsBelgianDate(),
                Tot = request.Tot!.Value.FormatAsBelgianDate(),
            },
        ];

    public static Lidmaatschap[] MapLidmaatschappen(WijzigLidmaatschapRequest request, string vCode, string andereVereniging, int lidmaatschapId)
        =>
        [
            new Lidmaatschap()
            {
                id = JsonLdType.Lidmaatschap.CreateWithIdValues(
                    vCode, lidmaatschapId.ToString()),
                type = JsonLdType.Lidmaatschap.Type,
                AndereVereniging = andereVereniging,
                Beschrijving = request.Beschrijving,
                Identificatie = request.Identificatie,
                Van = request.Van!.Value.FormatAsBelgianDate(),
                Tot = request.Tot!.Value.FormatAsBelgianDate(),
            },
        ];
}
