namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens.Beheer.Zoeken;

using Admin.Api.Verenigingen.Search.ResponseModels;
using Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Formats;
using JsonLdContext;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using Vereniging;
using KellermanSoftware.CompareNetObjects;
using NodaTime;

using Xunit;
using Vereniging = Admin.Api.Verenigingen.Search.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<WijzigBasisgegevensTestContext, WijzigBasisgegevensRequest, SearchVerenigingenResponse>
{
    private readonly WijzigBasisgegevensTestContext _testContext;

    public Returns_SearchVerenigingenResponse(WijzigBasisgegevensTestContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Verenigingen.Single().ShouldCompare(new Vereniging
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = Request.Doelgroep.Minimumleeftijd.Value,
                Maximumleeftijd = Request.Doelgroep.Maximumleeftijd.Value,
            },
            VCode = _testContext.VCode,
            KorteNaam = Request.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = Request.Naam,
            Startdatum = Instant.FromDateTimeOffset(
                new DateTimeOffset(Request.Startdatum.Value.ToDateTime(new TimeOnly(12, 0, 0)))
            ).FormatAsBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            HoofdactiviteitenVerenigingsloket = BeheerZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(Request.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = BeheerZoekResponseMapper.MapWerkingsgebieden(Request.Werkingsgebieden),
            Locaties = BeheerZoekResponseMapper.MapLocaties(_testContext.RegistratieData.Locaties, _testContext.VCode),
            Sleutels = BeheerZoekResponseMapper.MapSleutels(_testContext.VCode),
            Lidmaatschappen = [],
            Links = new VerenigingLinks()
            {
                Detail = new Uri($"{_testContext.AdminApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerZoeken($"vCode:{_testContext.VCode}");
}
