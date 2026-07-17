namespace AssociationRegistry.Test.E2E.When_Zet_Vereniging_In_Stopzetting.Publiek.Detail;

using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Formats;
using AssociationRegistry.Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using When_Wijzig_Basisgegevens;
using Xunit;
using DoelgroepResponse = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.DoelgroepResponse;
using Vereniging = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Verenigingstype;

[Collection(nameof(ZetVerenigingInStopzettingCollection))]
public class Returns_DetailResponse : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly ZetVerenigingInStopzettingContext _testContext;

    public Returns_DetailResponse(ZetVerenigingInStopzettingContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<PubliekVerenigingDetailResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.PublicApiHost.GetPubliekDetail(_testContext.VCode);

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(
            Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
            compareConfig: new ComparisonConfig { MaxMillisecondsDateDifference = 5000 }
        );
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging() =>
        Response.Vereniging.ShouldCompare(
            new Vereniging
            {
                type = JsonLdType.FeitelijkeVereniging.Type,
                Doelgroep = new DoelgroepResponse
                {
                    type = JsonLdType.Doelgroep.Type,
                    id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                    Minimumleeftijd = _testContext
                        .Scenario
                        .FeitelijkeVerenigingWerdGeregistreerd
                        .Doelgroep
                        .Minimumleeftijd,
                    Maximumleeftijd = _testContext
                        .Scenario
                        .FeitelijkeVerenigingWerdGeregistreerd
                        .Doelgroep
                        .Maximumleeftijd,
                },
                VCode = _testContext.VCode,
                KorteBeschrijving = _testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.KorteBeschrijving,
                KorteNaam = _testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.KorteNaam,
                Verenigingstype = new Verenigingstype
                {
                    Code = DecentraalBeheer.Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                    Naam = DecentraalBeheer.Vereniging.Verenigingstype.FeitelijkeVereniging.Naam,
                },
                Naam = _testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam,
                Startdatum = DateOnly.FromDateTime(DateTime.Now),
                Status = VerenigingStatus.Actief,
                Contactgegevens = PubliekDetailResponseMapper.MapContactgegevens(
                    _testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens,
                    _testContext.VCode
                ),
                HoofdactiviteitenVerenigingsloket = PubliekDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(
                    _testContext
                        .Scenario.FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket.Select(x =>
                            x.Code
                        )
                        .ToArray()
                ),
                Werkingsgebieden = [],
                Locaties = PubliekDetailResponseMapper.MapLocaties(
                    _testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties,
                    _testContext.VCode
                ),
                Relaties = [],
                Sleutels = PubliekDetailResponseMapper.MapSleutels(_testContext.VCode),
            },
            compareConfig: AdminDetailComparisonConfig.Instance
        );
}
