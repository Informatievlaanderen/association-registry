namespace AssociationRegistry.Test.E2E.Erkenningen.When_Hef_Schorsings_Erkenning_Op.Publiek.Detail;

using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Framework.AlbaHost;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Public.ProjectionHost.Constants;
using Xunit;
using Erkenning = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Erkenning;
using IpdcProduct = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.IpdcProduct;

[Collection(nameof(HefSchorsingErkenningOpCollection))]
public class Returns_Detail : IAsyncLifetime
{
    private readonly HefSchorsingErkenningOpContext _testContext;

    public Returns_Detail(HefSchorsingErkenningOpContext testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void JsonContentMatches()
    {
        Response
            .Vereniging.Erkenningen.Should()
            .BeEquivalentTo([
                new Erkenning
                {
                    id = JsonLdType.Erkenning.CreateWithIdValues(
                        _testContext.VCode.Value,
                        _testContext.Scenario.ErkenningWerdGeregistreerd.ErkenningId.ToString()
                    ),
                    type = JsonLdType.Erkenning.Type,
                    ErkenningId = _testContext.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                    GeregistreerdDoor = new GegevensInitiatorErkenning
                    {
                        OvoCode = _testContext.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
                        Naam = _testContext.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.Naam,
                    },
                    IpdcProduct = new IpdcProduct
                    {
                        Nummer = _testContext.Scenario.ErkenningWerdGeregistreerd.IpdcProduct.Nummer,
                        Naam = _testContext.Scenario.ErkenningWerdGeregistreerd.IpdcProduct.Naam,
                    },
                    Startdatum = _testContext.Scenario.ErkenningWerdGeregistreerd.Startdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Einddatum = _testContext.Scenario.ErkenningWerdGeregistreerd.Einddatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Hernieuwingsdatum = _testContext.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    HernieuwingsUrl = _testContext.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = ErkenningStatus.Bepaal(
                        ErkenningsPeriode.Create(
                            _testContext.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                            _testContext.Scenario.ErkenningWerdGeregistreerd.Einddatum
                        ),
                        DateOnly.FromDateTime(DateTime.Now)
                    ),
                },
            ]);
    }

    public PubliekVerenigingDetailResponse Response { get; set; }

    public async ValueTask InitializeAsync()
    {
        Response = await _testContext.ApiSetup.PublicApiHost.GetPubliekDetail(_testContext.VCode);
    }

    public async ValueTask DisposeAsync() { }
}
