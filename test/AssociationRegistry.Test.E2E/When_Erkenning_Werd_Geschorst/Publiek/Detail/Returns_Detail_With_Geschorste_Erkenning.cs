namespace AssociationRegistry.Test.E2E.When_Erkenning_Werd_Geschorst.Publiek.Detail;

using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Public.ProjectionHost.Constants;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using FluentAssertions;
using Xunit;
using Erkenning = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Erkenning;
using IpdcProduct = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.IpdcProduct;

[Collection(nameof(ErkenningWerdGeschorstCollection))]
public class Returns_Detail_With_Geschorste_Erkenning : IAsyncLifetime
{
    private readonly ErkenningWerdGeschorstContext _werdGeschorstContext;

    public Returns_Detail_With_Geschorste_Erkenning(ErkenningWerdGeschorstContext werdGeschorstContext)
    {
        _werdGeschorstContext = werdGeschorstContext;
    }

    [Fact]
    public void JsonContentMatches()
    {
        var geregistreerd = _werdGeschorstContext.Scenario.ErkenningWerdGeregistreerd;

        Response
           .Vereniging.Erkenningen.Should()
           .BeEquivalentTo([
                new Erkenning
                {
                    type = JsonLdType.Erkenning.Type,
                    id = JsonLdType.Erkenning.CreateWithIdValues(
                        _werdGeschorstContext.VCode,
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
                    RedenSchorsing = _werdGeschorstContext.CommandRequest.RedenSchorsing,
                    Status = ErkenningStatus.Geschorst,
                }
            ]);
    }

    public PubliekVerenigingDetailResponse Response { get; set; }

    public async ValueTask InitializeAsync()
    {
        Response = await _werdGeschorstContext.ApiSetup.PublicApiHost.GetPubliekDetail(_werdGeschorstContext.VCode);
    }

    public async ValueTask DisposeAsync() { }
}
