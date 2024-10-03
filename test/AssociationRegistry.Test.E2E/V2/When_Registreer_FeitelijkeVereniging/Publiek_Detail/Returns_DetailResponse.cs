namespace AssociationRegistry.Test.E2E.V2.When_Registreer_FeitelijkeVereniging.Publiek_Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.Schema.Constants;
using Formats;
using JsonLdContext;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.TestClasses;
using Vereniging;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Xunit;
using Contactgegeven = Public.Api.Verenigingen.Detail.ResponseModels.Contactgegeven;
using DoelgroepResponse = Public.Api.Verenigingen.Detail.ResponseModels.DoelgroepResponse;
using GestructureerdeIdentificator = Public.Api.Verenigingen.Detail.ResponseModels.GestructureerdeIdentificator;
using HoofdactiviteitVerenigingsloket = Public.Api.Verenigingen.Detail.ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = Public.Api.Verenigingen.Detail.ResponseModels.Locatie;
using Relatie = Public.Api.Verenigingen.Detail.ResponseModels.Relatie;
using Sleutel = Public.Api.Verenigingen.Detail.ResponseModels.Sleutel;
using Vereniging = Public.Api.Verenigingen.Detail.ResponseModels.Vereniging;
using VerenigingsType = Public.Api.Verenigingen.Detail.ResponseModels.VerenigingsType;
[Collection(FullBlownApiCollection.Name)]
public class Returns_DetailResponse : End2EndTest<RegistreerFeitelijkeVerenigingContext, RegistreerFeitelijkeVerenigingRequest, PubliekVerenigingDetailResponse>
{
    private readonly RegistreerFeitelijkeVerenigingContext _context;

    public Returns_DetailResponse(RegistreerFeitelijkeVerenigingContext context) : base(context)
    {
        _context = context;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11004/v1/contexten/publiek/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
                                                               compareConfig: new ComparisonConfig
                                                                   { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public async Task WithFeitelijkeVereniging()
        => Response.Vereniging.ShouldCompare(new Vereniging
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_context.VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = _context.VCode,
            KorteBeschrijving = Request.KorteBeschrijving,
            KorteNaam = Request.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = Request.Naam,
            Startdatum = DateOnly.FromDateTime(DateTime.Now),
            Status = VerenigingStatus.Actief,
            Contactgegevens = MapLocaties(Request.Contactgegevens, _context.VCode),
            HoofdactiviteitenVerenigingsloket = MapHoofdactiviteitenVerenigingsloket(Request.HoofdactiviteitenVerenigingsloket),
            Locaties = MapLocaties(Request.Locaties, _context.VCode),
            Relaties = MapRelaties([], _context.VCode),
            Sleutels = MapSleutels(Request, _context.VCode),
        }, compareConfig: AdminDetailComparisonConfig.Instance);

    private static Sleutel[] MapSleutels(RegistreerFeitelijkeVerenigingRequest request, string vCode)
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

    private static Relatie[] MapRelaties(Relatie[] relaties, string vCode)
    {
        return relaties.Select((x, i) => new Relatie
        {
            AndereVereniging = x.AndereVereniging,
            Relatietype = x.Relatietype,
        }).ToArray();
    }

    private static Contactgegeven[] MapLocaties(ToeTeVoegenContactgegeven[] toeTeVoegenContactgegevens, string vCode)
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

    private static Locatie[] MapLocaties(ToeTeVoegenLocatie[] toeTeVoegenLocaties, string vCode)
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

    private static HoofdactiviteitVerenigingsloket[] MapHoofdactiviteitenVerenigingsloket(
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


    public override Func<IApiSetup, PubliekVerenigingDetailResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekDetail(_context.VCode);
}
