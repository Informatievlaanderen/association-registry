namespace AssociationRegistry.Test.E2E.When_Erkenning_Werd_Geschorst.Beheer.Detail;

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

[Collection(nameof(SchorsErkenningCollection))]
public class Returns_Detail_With_Geschorste_Erkenning : End2EndTest<DetailVerenigingResponse>
{
    private readonly ErkenningWerdGeschorstContext _testWerdGeschorstContext;

    public Returns_Detail_With_Geschorste_Erkenning(ErkenningWerdGeschorstContext testWerdGeschorstContext)
        : base(testWerdGeschorstContext.ApiSetup)
    {
        _testWerdGeschorstContext = testWerdGeschorstContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerDetail(
            setup.AdminHttpClient,
            _testWerdGeschorstContext.VCode,
            new RequestParameters().WithExpectedSequence(_testWerdGeschorstContext.CommandResult.Sequence)
        );

    [Fact]
    public void JsonContentMatches()
    {
        var geregistreerd = _testWerdGeschorstContext.Scenario.ErkenningWerdGeregistreerd;

        Response
           .Vereniging.Erkenningen.Should()
           .BeEquivalentTo([
                new Erkenning
                {
                    type = JsonLdType.Erkenning.Type,
                    id = JsonLdType.Erkenning.CreateWithIdValues(
                        _testWerdGeschorstContext.VCode,
                        geregistreerd.ErkenningId.ToString()
                    ),
                    ErkenningId = geregistreerd.ErkenningId,
                    GeregistreerdDoor = new GegevensInitiatorErkenning
                    {
                        OvoCode = geregistreerd.GeregistreerdDoor.OvoCode,
                        Naam = geregistreerd.GeregistreerdDoor.Naam,
                    },
                    IpdcProduct = new IpdcProduct
                    {
                        Nummer = geregistreerd.IpdcProduct.Nummer,
                        Naam = geregistreerd.IpdcProduct.Naam,
                    },
                    Startdatum = geregistreerd.Startdatum?.ToString(WellknownFormats.DateOnly),
                    Einddatum = geregistreerd.Einddatum?.ToString(WellknownFormats.DateOnly),
                    Hernieuwingsdatum = geregistreerd.Hernieuwingsdatum?.ToString(WellknownFormats.DateOnly),
                    HernieuwingsUrl = geregistreerd.HernieuwingsUrl,
                    RedenSchorsing = _testWerdGeschorstContext.CommandRequest.Erkenning.RedenSchorsing,
                    Status = ErkenningStatus.Geschorst,
                }
            ]);
    }
}
