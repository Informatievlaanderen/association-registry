namespace AssociationRegistry.Test.E2E.Framework.Mappers;

using Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Events;
using JsonLdContext;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Vereniging;
using Contactgegeven = Public.Api.Verenigingen.Detail.ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = Public.Api.Verenigingen.Detail.ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = Public.Api.Verenigingen.Detail.ResponseModels.Locatie;
using Werkingsgebied = Public.Api.Verenigingen.Detail.ResponseModels.Werkingsgebied;

public class PubliekDetailResponseMapper
{
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

    public static Relatie[] MapRelaties(Relatie[] relaties, string vCode)
    {
        return relaties.Select((x, i) => new Relatie
        {
            AndereVereniging = x.AndereVereniging,
            Relatietype = x.Relatietype,
        }).ToArray();
    }

    public static Contactgegeven[] MapContactgegevens(Registratiedata.Contactgegeven[] toeTeVoegenContactgegevens, string vCode)
    {
        return toeTeVoegenContactgegevens.Select((x, i) => new Contactgegeven
        {
            id = JsonLdType.Contactgegeven.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Contactgegeven.Type,
            Contactgegeventype = x.Contactgegeventype,
            Waarde = x.Waarde,
            Beschrijving = x.Beschrijving!,
            IsPrimair = x.IsPrimair,
        }).ToArray();
    }

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
