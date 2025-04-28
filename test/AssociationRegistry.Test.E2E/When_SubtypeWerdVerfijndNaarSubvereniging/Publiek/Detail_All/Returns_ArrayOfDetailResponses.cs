namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarSubvereniging.Publiek.Detail_All;

using Admin.Api.Verenigingen.Subtype.RequestModels;
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
using DoelgroepResponse = Public.Api.Verenigingen.Detail.ResponseModels.DoelgroepResponse;
using SubverenigingVan = Public.Api.Verenigingen.Detail.ResponseModels.SubverenigingVan;
using Vereniging = Public.Api.Verenigingen.Detail.ResponseModels.Vereniging;
using Verenigingssubtype = Public.Api.Verenigingen.Detail.ResponseModels.Verenigingssubtype;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Public.Api.Verenigingen.Detail.ResponseModels.Verenigingstype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_ArrayOfDetailResponses : End2EndTest<VerfijnSubtypeNaarSubverenigingContext, WijzigSubtypeRequest, PubliekVerenigingDetailResponse>
{
    private readonly VerfijnSubtypeNaarSubverenigingContext _context;

    public override Func<IApiSetup, PubliekVerenigingDetailResponse> GetResponse =>
        setup => setup.PublicApiHost
                      .GetPubliekDetailAll()
                      .FindVereniging(TestContext.VCode);

    public Returns_ArrayOfDetailResponses(VerfijnSubtypeNaarSubverenigingContext testContext) : base(testContext)
    {
        _context = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/detail-all-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
                                                                        compareConfig: new ComparisonConfig
                                                                            { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public void WithFeitelijkeVereniging()
        => Response.Vereniging.ShouldCompare(new Vereniging
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(TestContext.VCode),
                Minimumleeftijd = _context.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Doelgroep.Minimumleeftijd,
                Maximumleeftijd = _context.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Doelgroep.Maximumleeftijd,
            },
            VCode = TestContext.VCode,
            KorteBeschrijving = _context.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.KorteBeschrijving,
            KorteNaam = _context.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.KorteNaam,
            Verenigingstype = new Verenigingstype
            {
                Code = AssociationRegistry.Vereniging.Verenigingstype.VZER.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingstype.VZER.Naam,
            },
            Verenigingssubtype = new Verenigingssubtype
            {
                Code = VerenigingssubtypeCode.Subvereniging.Code,
                Naam = VerenigingssubtypeCode.Subvereniging.Naam,
            },
            SubverenigingVan = new SubverenigingVan()
            {
                AndereVereniging = _context.Request.AndereVereniging!,
                Beschrijving = _context.Request.Beschrijving!,
                Identificatie = _context.Request.Identificatie!,
            },
            Naam = _context.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Naam,
            Startdatum = _context.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Startdatum.Value,
            Status = VerenigingStatus.Actief,
            Contactgegevens = PubliekDetailResponseMapper.MapContactgegevens(_context.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Contactgegevens, TestContext.VCode),
            HoofdactiviteitenVerenigingsloket = PubliekDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(_context.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.HoofdactiviteitenVerenigingsloket.Select(x => x.Code).ToArray()),
            Werkingsgebieden = [],
            Locaties = PubliekDetailResponseMapper.MapLocaties(_context.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties, TestContext.VCode),
            Relaties = [],
            Sleutels = PubliekDetailResponseMapper.MapSleutels(TestContext.VCode),
        }, compareConfig: AdminDetailComparisonConfig.Instance);
}
