namespace AssociationRegistry.Test.E2E.V2.When_Wijzig_Basisgegevens.Publiek_Detail;

using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Admin.Schema.Constants;
using Events;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Vereniging;
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
using Werkingsgebied = Public.Api.Verenigingen.Detail.ResponseModels.Werkingsgebied;

[Collection(FullBlownApiCollection.Name)]
public class Returns_DetailResponse : End2EndTest<WijzigBasisgegevensTestContext, WijzigBasisgegevensRequest, PubliekVerenigingDetailResponse>
{
    private readonly WijzigBasisgegevensTestContext _testContext;

    public Returns_DetailResponse(WijzigBasisgegevensTestContext testContext) : base(testContext)
    {
        _testContext = testContext;
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
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = _testContext.VCode,
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
            Contactgegevens = PubliekDetailResponseMapper.MapContactgegevens(_testContext.RegistratieData.Contactgegevens, _testContext.VCode),
            HoofdactiviteitenVerenigingsloket = PubliekDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(Request.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = PubliekDetailResponseMapper.MapWerkingsgebieden(Request.Werkingsgebieden),
            Locaties = PubliekDetailResponseMapper.MapLocaties(_testContext.RegistratieData.Locaties, _testContext.VCode),
            Relaties = PubliekDetailResponseMapper.MapRelaties([], _testContext.VCode),
            Sleutels = PubliekDetailResponseMapper.MapSleutels(Request, _testContext.VCode),
        }, compareConfig: AdminDetailComparisonConfig.Instance);

    public override Func<IApiSetup, PubliekVerenigingDetailResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekDetail(_testContext.VCode);
}
