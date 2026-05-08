namespace AssociationRegistry.Test.E2E.Erkenningen.When_Corrigeer_Reden_Schorsings_Erkenning.Publiek.Detail;

using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Public.ProjectionHost.Constants;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using FluentAssertions;
using Xunit;
using Erkenning = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Erkenning;
using IpdcProduct = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.IpdcProduct;

[Collection(nameof(CorrigeerSchorsingErkenningCollection))]
public class Returns_Detail : IAsyncLifetime
{
    private readonly CorrigeerSchorsingErkenningContext _testContext;

    public Returns_Detail(CorrigeerSchorsingErkenningContext testContext)
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
                    RedenSchorsing = _testContext.CommandRequest.RedenSchorsing,
                    Status = ErkenningStatus.Geschorst,
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
