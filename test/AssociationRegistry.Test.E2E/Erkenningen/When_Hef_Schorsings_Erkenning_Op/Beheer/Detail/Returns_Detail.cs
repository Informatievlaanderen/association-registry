namespace AssociationRegistry.Test.E2E.Erkenningen.When_Hef_Schorsings_Erkenning_Op.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Admin.ProjectionHost.Constants;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;
using Erkenning = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Erkenning;
using IpdcProduct = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.IpdcProduct;

[Collection(nameof(HefSchorsingErkenningOpCollection))]
public class Returns_Detail : End2EndTest<DetailVerenigingResponse>
{
    private readonly HefSchorsingErkenningOpContext _testContext;

    public Returns_Detail(HefSchorsingErkenningOpContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerDetail(
            setup.AdminHttpClient,
            _testContext.VCode,
            new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void JsonContentMatches()
    {
        Response
            .Vereniging.Erkenningen.Should()
            .BeEquivalentTo([
                new Erkenning
                {
                    type = JsonLdType.Erkenning.Type,
                    id = JsonLdType.Erkenning.CreateWithIdValues(
                        _testContext.VCode,
                        _testContext.Scenario.ErkenningWerdGeregistreerd.ErkenningId.ToString()
                    ),
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
}
