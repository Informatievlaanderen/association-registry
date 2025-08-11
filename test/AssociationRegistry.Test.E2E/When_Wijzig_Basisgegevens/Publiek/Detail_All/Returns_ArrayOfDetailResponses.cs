namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens.Publiek.Detail_All;

using Contracts.JsonLdContext;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Xunit;
using DoelgroepResponse = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.DoelgroepResponse;
using Vereniging = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Vereniging;
using Verenigingssubtype = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Verenigingssubtype;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Verenigingstype;

[Collection(nameof(WijzigBasisGegevensCollection))]
public class Returns_ArrayOfDetailResponses
    : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly WijzigBasisgegevensContext _testContext;

    public Returns_ArrayOfDetailResponses(WijzigBasisgegevensContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override PubliekVerenigingDetailResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost
                           .GetPubliekDetailAll(_testContext.CommandResult.Sequence)
                           .FindVereniging(_testContext.VCode);

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
    public void WithVerenigingZonderEigenRechtspersoonlijkheid()
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
            KorteBeschrijving = _testContext.CommandRequest.KorteBeschrijving,
            KorteNaam = _testContext.CommandRequest.KorteNaam,
            Verenigingstype = new Verenigingstype
            {
                Code = DecentraalBeheer.Vereniging.Verenigingstype.VZER.Code,
                Naam = DecentraalBeheer.Vereniging.Verenigingstype.VZER.Naam,
            },
            Verenigingssubtype = new Verenigingssubtype
            {
                Code = string.Empty,
                Naam = string.Empty,
            },
            SubverenigingVan = null,
            Naam = _testContext.CommandRequest.Naam,
            Startdatum = DateOnly.FromDateTime(DateTime.Now),
            Status = VerenigingStatus.Actief,
            Contactgegevens = PubliekDetailResponseMapper.MapContactgegevens(_testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens, _testContext.VCode),
            HoofdactiviteitenVerenigingsloket = PubliekDetailResponseMapper.MapHoofdactiviteitenVerenigingsloket(_testContext.CommandRequest.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = PubliekDetailResponseMapper.MapWerkingsgebieden(_testContext.CommandRequest.Werkingsgebieden),
            Locaties = PubliekDetailResponseMapper.MapLocaties(_testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties, _testContext.VCode),
            Relaties = [],
            Sleutels = PubliekDetailResponseMapper.MapSleutels(_testContext.VCode),
        }, compareConfig: AdminDetailComparisonConfig.Instance);
}
