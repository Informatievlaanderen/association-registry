namespace AssociationRegistry.Test.E2E.When_Registreer_Vereniging.PubliekDetail;

using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Alba;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using Formats;
using Framework.AlbaHost;
using Framework.Comparison;
using Framework.TestClasses;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Public.Schema.Constants;
using Vereniging;
using Xunit;
using HoofdactiviteitVerenigingsloket = Vereniging.HoofdactiviteitVerenigingsloket;
using Relatie = Admin.Api.Verenigingen.Detail.ResponseModels.Relatie;
using Vereniging = Public.Api.Verenigingen.Detail.ResponseModels.Vereniging;

[Collection(nameof(RegistreerVerenigingContext))]
public class Returns_DetailResponse(RegistreerVerenigingContext context)
    : End2EndTest<RegistreerVerenigingContext, RegistreerFeitelijkeVerenigingRequest, PubliekVerenigingDetailResponse>(context)
{
    protected override Func<IAlbaHost, PubliekVerenigingDetailResponse> GetResponse =>
        host => host.GetPubliekDetail(VCode);

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare( Instant.FromDateTimeOffset(DateTimeOffset.Now).ToBelgianDate(),
                                                               compareConfig: new ComparisonConfig
                                                                   { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public async Task WithVereniging()
        => Response.Vereniging.ShouldCompare(new Vereniging
        {

            type = JsonLdType.FeitelijkeVereniging.Type,
            Doelgroep = new Public.Api.Verenigingen.Detail.ResponseModels.DoelgroepResponse()
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = VCode,
            KorteBeschrijving = Request.KorteBeschrijving,
            KorteNaam = Request.KorteNaam,
            Verenigingstype = new Public.Api.Verenigingen.Detail.ResponseModels.VerenigingsType()
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = Request.Naam,
            Startdatum = DateOnly.FromDateTime(DateTime.Now),
            Status = VerenigingStatus.Actief,
            Contactgegevens = MapLocaties(Request.Contactgegevens, VCode),
            HoofdactiviteitenVerenigingsloket = MapHoofdactiviteitenVerenigingsloket(Request.HoofdactiviteitenVerenigingsloket),
            Locaties = MapLocaties(Request.Locaties, VCode),
            Relaties = MapRelaties([], VCode),
            Sleutels = MapSleutels(Request, VCode),
        }, compareConfig: AdminDetailComparisonConfig.Instance);

    private static Public.Api.Verenigingen.Detail.ResponseModels.Sleutel[] MapSleutels(RegistreerFeitelijkeVerenigingRequest request, string vCode)
        =>
        [
            new Public.Api.Verenigingen.Detail.ResponseModels.Sleutel
            {
                Bron = Sleutelbron.VR.Waarde,
                id = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                type = JsonLdType.Sleutel.Type,
                Waarde = vCode,
                CodeerSysteem = CodeerSysteem.VR,
                GestructureerdeIdentificator = new Public.Api.Verenigingen.Detail.ResponseModels.GestructureerdeIdentificator
                {
                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                    type = JsonLdType.GestructureerdeSleutel.Type,
                    Nummer = vCode,
                },
            },
        ];

    private static Public.Api.Verenigingen.Detail.ResponseModels.Relatie[] MapRelaties(Relatie[] relaties, string vCode)
    {
        return relaties.Select((x, i) => new Public.Api.Verenigingen.Detail.ResponseModels.Relatie
        {
            AndereVereniging = new Public.Api.Verenigingen.Detail.ResponseModels.Relatie.GerelateerdeVereniging()
            {
                Detail = x.AndereVereniging.Detail,
                Naam = x.AndereVereniging.Naam,
                KboNummer = x.AndereVereniging.KboNummer,
                VCode = x.AndereVereniging.VCode,
            } ,
            Relatietype = x.Relatietype,
        }).ToArray();
    }

    private static Public.Api.Verenigingen.Detail.ResponseModels.Contactgegeven[] MapLocaties(ToeTeVoegenContactgegeven[] toeTeVoegenContactgegevens, string vCode)
    {
        return toeTeVoegenContactgegevens.Select((x, i) => new Public.Api.Verenigingen.Detail.ResponseModels.Contactgegeven
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

    private static Public.Api.Verenigingen.Detail.ResponseModels.Locatie[] MapLocaties(ToeTeVoegenLocatie[] toeTeVoegenLocaties, string vCode)
    {
        return toeTeVoegenLocaties.Select((x, i) => new Public.Api.Verenigingen.Detail.ResponseModels.Locatie
        {
            id = JsonLdType.Locatie.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Locatie.Type,
            Locatietype = x.Locatietype,
            Naam = x.Naam,
            IsPrimair = x.IsPrimair,
        }).ToArray();
    }

    private static Public.Api.Verenigingen.Detail.ResponseModels.HoofdactiviteitVerenigingsloket[] MapHoofdactiviteitenVerenigingsloket(
        string[] hoofdactiviteitenVerenigingsloket)
    {
        return hoofdactiviteitenVerenigingsloket.Select(x =>
        {
            var hoofdactiviteitVerenigingsloket = HoofdactiviteitVerenigingsloket.Create(x);

            return new Public.Api.Verenigingen.Detail.ResponseModels.HoofdactiviteitVerenigingsloket
            {
                Code = hoofdactiviteitVerenigingsloket.Code,
                Naam = hoofdactiviteitVerenigingsloket.Naam,
                id = JsonLdType.Hoofdactiviteit.CreateWithIdValues(hoofdactiviteitVerenigingsloket.Code),
                type = JsonLdType.Hoofdactiviteit.Type,
            };
        }).ToArray();
    }
}
