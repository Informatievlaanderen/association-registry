namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging.Publiek.Detail.With_Header;

using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using Formats;
using JsonLdContext;
using AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using Vereniging;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Vereniging.Mappers;
using Xunit;
using DoelgroepResponse = Public.Api.Verenigingen.Detail.ResponseModels.DoelgroepResponse;
using Vereniging = Public.Api.Verenigingen.Detail.ResponseModels.Vereniging;
using Verenigingssubtype = Public.Api.Verenigingen.Detail.ResponseModels.Verenigingssubtype;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Public.Api.Verenigingen.Detail.ResponseModels.Verenigingstype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VZER_DetailResponse : End2EndTest<RegistreerFeitelijkeVerenigingTestContext, RegistreerFeitelijkeVerenigingRequest, PubliekVerenigingDetailResponse>
{
    private readonly RegistreerFeitelijkeVerenigingTestContext _testContext;

    public Returns_VZER_DetailResponse(RegistreerFeitelijkeVerenigingTestContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/detail-vereniging-context.json");
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
            Verenigingstype = new Verenigingstype
            {
                Code = AssociationRegistry.Vereniging.Verenigingstype.VZER.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingstype.VZER.Naam,
            },
            Verenigingssubtype = VerenigingssubtypeCodering.Default.Map<Verenigingssubtype>(),
            Naam = Request.Naam,
            Startdatum = DateOnly.FromDateTime(DateTime.Now),
            Status = VerenigingStatus.Actief,
            Contactgegevens = PubliekDetailResponseMapper.MapContactgegevens(Request.Contactgegevens, _testContext.VCode),
            HoofdactiviteitenVerenigingsloket = PubliekDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(Request.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = PubliekDetailResponseMapper.MapWerkingsgebieden(Request.Werkingsgebieden),
            Locaties = PubliekDetailResponseMapper.MapLocaties(Request.Locaties, _testContext.VCode),
            Relaties = [],
            Sleutels = PubliekDetailResponseMapper.MapSleutels(_testContext.VCode),
        }, compareConfig: AdminDetailComparisonConfig.Instance);


    public override Func<IApiSetup, PubliekVerenigingDetailResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekDetailWithHeader(setup.SuperAdminHttpClient, _testContext.VCode, _testContext.RequestResult.Sequence).GetAwaiter().GetResult();
}
