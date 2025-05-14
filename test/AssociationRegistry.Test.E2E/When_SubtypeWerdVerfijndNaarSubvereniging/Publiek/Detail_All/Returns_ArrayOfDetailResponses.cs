namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarSubvereniging.Publiek.Detail_All;

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

[Collection(nameof(VerfijnSubtypeNaarSubverenigingCollection))]
public class Returns_Detail : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly VerfijnSubtypeNaarSubverenigingContext _testContext;

    public Returns_Detail(VerfijnSubtypeNaarSubverenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override PubliekVerenigingDetailResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost
                 .GetPubliekDetailAll()
                 .FindVereniging(_testContext.CommandResult.VCode);

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
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Doelgroep.Minimumleeftijd,
                Maximumleeftijd = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Doelgroep.Maximumleeftijd,
            },
            VCode = _testContext.VCode,
            KorteBeschrijving = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.KorteBeschrijving,
            KorteNaam = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.KorteNaam,
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
                AndereVereniging = _testContext.CommandRequest.AndereVereniging!,
                Beschrijving = _testContext.CommandRequest.Beschrijving!,
                Identificatie = _testContext.CommandRequest.Identificatie!,
            },
            Naam = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Naam,
            Startdatum = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Startdatum.Value,
            Status = VerenigingStatus.Actief,
            Contactgegevens = PubliekDetailResponseMapper.MapContactgegevens(_testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Contactgegevens, _testContext.VCode),
            HoofdactiviteitenVerenigingsloket = PubliekDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(_testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.HoofdactiviteitenVerenigingsloket.Select(x => x.Code).ToArray()),
            Werkingsgebieden = [],
            Locaties = PubliekDetailResponseMapper.MapLocaties(_testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties, _testContext.VCode),
            Relaties = [],
            Sleutels = PubliekDetailResponseMapper.MapSleutels(_testContext.VCode),
        }, compareConfig: AdminDetailComparisonConfig.Instance);
}
