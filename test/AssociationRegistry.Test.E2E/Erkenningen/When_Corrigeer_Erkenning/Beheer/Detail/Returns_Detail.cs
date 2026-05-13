namespace AssociationRegistry.Test.E2E.Erkenningen.When_Corrigeer_Erkenning.Beheer.Detail;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Admin.ProjectionHost.Constants;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using Xunit;
using Erkenning = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Erkenning;
using IpdcProduct = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.IpdcProduct;

[Collection(nameof(CorrigeerErkenningCollection))]
public class Returns_Detail : End2EndTest<DetailVerenigingResponse>
{
    private readonly CorrigeerErkenningContext _testContext;

    public Returns_Detail(CorrigeerErkenningContext testContext)
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
                    Startdatum = _testContext.CommandRequest.Startdatum.Value.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Einddatum = _testContext.CommandRequest.Einddatum.Value.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Hernieuwingsdatum = _testContext.CommandRequest.Hernieuwingsdatum.Value.ToString(
                        WellknownFormats.DateOnly
                    ),
                    HernieuwingsUrl = _testContext.CommandRequest.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = ErkenningStatus.Bepaal(
                        ErkenningsPeriode.Create(
                            _testContext.CommandRequest.Startdatum.Value,
                            _testContext.CommandRequest.Einddatum.Value
                        ),
                        DateOnly.FromDateTime(DateTime.Now)
                    ),
                },
            ]);
    }
}
