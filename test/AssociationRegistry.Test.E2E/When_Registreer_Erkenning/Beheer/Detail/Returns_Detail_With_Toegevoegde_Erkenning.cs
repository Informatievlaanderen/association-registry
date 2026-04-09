namespace AssociationRegistry.Test.E2E.When_Registreer_Erkenning.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

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
           .BeEquivalentTo(
                [
                    new Erkenning
                    {
                        type = JsonLdType.Erkenning.Type,
                        id = JsonLdType.Erkenning.CreateWithIdValues(
                            _testContext.VCode, "1"
                        ),
                        ErkenningId = 1,
                        VCode = _testContext.VCode.Value,
                        GeregistreerdDoor = new GegevensInitiatorErkenning
                        {
                            OvoCode = "OVO000001",
                            Naam = string.Empty,
                        },
                        IpdcProduct = new IpdcProduct
                        {
                            Nummer = _testContext.CommandRequest.Erkenning.IpdcProductNummer,
                            Naam = string.Empty,
                        },
                        Startdatum = _testContext.CommandRequest.Erkenning.Startdatum,
                        Einddatum = _testContext.CommandRequest.Erkenning.Einddatum,
                        Hernieuwingsdatum = _testContext.CommandRequest.Erkenning.Hernieuwingsdatum,
                        HernieuwingsUrl = _testContext.CommandRequest.Erkenning.HernieuwingsUrl,
                        RedenSchorsing = string.Empty,
                    },
                ]
            );
    }
}
