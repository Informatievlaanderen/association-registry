namespace AssociationRegistry.Test.E2E.When_Registreer_Erkenning.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;
using Erkenning = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Erkenning;
using IpdcProduct = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.IpdcProduct;

[Collection(nameof(RegistreerErkenningCollection))]
public class Returns_Detail_With_Toegevoegde_Erkenning : End2EndTest<DetailVerenigingResponse>
{
    private readonly RegistreerErkenningContext _testContext;

    public Returns_Detail_With_Toegevoegde_Erkenning(RegistreerErkenningContext testContext)
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
                    id = JsonLdType.Erkenning.CreateWithIdValues(_testContext.VCode, "1"),
                    ErkenningId = 1,
                    GeregistreerdDoor = new GegevensInitiatorErkenning
                    {
                        OvoCode = "OVO000001",
                        Naam = "De LijnInfo", // See wiremock
                    },
                    IpdcProduct = new IpdcProduct
                    {
                        Nummer = _testContext.CommandRequest.Erkenning.IpdcProductNummer,
                        Naam = "Gemeentelijke premie voor inbraakpreventie", // See wiremock
                    },
                    Startdatum = _testContext.CommandRequest.Erkenning.Startdatum,
                    Einddatum = _testContext.CommandRequest.Erkenning.Einddatum,
                    Hernieuwingsdatum = _testContext.CommandRequest.Erkenning.Hernieuwingsdatum,
                    HernieuwingsUrl = _testContext.CommandRequest.Erkenning.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = ErkenningStatus.Bepaal(
                        ErkenningsPeriode.Create(
                            _testContext.CommandRequest.Erkenning.Startdatum,
                            _testContext.CommandRequest.Erkenning.Einddatum
                        ),
                        DateOnly.FromDateTime(DateTime.Now)
                    ),
                },
            ]);
    }
}
