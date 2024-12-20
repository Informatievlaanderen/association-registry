namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo.Publiek.Detail;

using Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using Admin.Schema.Constants;
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
using Xunit;
using Vereniging = Public.Api.Verenigingen.Detail.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;

[Collection(FullBlownApiCollection.Name)]
public class Returns_DetailResponse : End2EndTest<WijzigBasisgegevensKboTestContext, WijzigBasisgegevensRequest,
    PubliekVerenigingDetailResponse>
{
    private readonly WijzigBasisgegevensKboTestContext _testContext;

    public Returns_DetailResponse(WijzigBasisgegevensKboTestContext testContext) : base(testContext)
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
    public async Task WithVerenigingMetRechtspersoonlijkheid()
        => Response.Vereniging.ShouldCompare(new Vereniging
        {
            type = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = _testContext.VCode,
            KorteBeschrijving = Request.KorteBeschrijving,
            KorteNaam = _testContext.RegistratieData.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.VZW.Code,
                Naam = Verenigingstype.VZW.Naam,
            },
            Naam = _testContext.RegistratieData.Naam,
            Roepnaam = Request.Roepnaam,
            Startdatum = DateOnly.FromDateTime(DateTime.Now),
            Status = VerenigingStatus.Actief,
            Contactgegevens = [],
            HoofdactiviteitenVerenigingsloket = PubliekDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(Request.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = PubliekDetailResponseMapper.MapWerkingsgebieden(Request.Werkingsgebieden),
            Locaties = [],
            Relaties = [],
            Sleutels = PubliekDetailResponseMapper.MapSleutels(_testContext.VCode, _testContext.RegistratieData.KboNummer),
        }, compareConfig: AdminDetailComparisonConfig.Instance);

    public override Func<IApiSetup, PubliekVerenigingDetailResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekDetail(_testContext.VCode);
}
