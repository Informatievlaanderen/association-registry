namespace AssociationRegistry.Test.E2E.Erkenningen.When_Schors_Erkenning.Publiek.Detail;

using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Public.ProjectionHost.Constants;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using FluentAssertions;
using When_Erkenning_Werd_Geschorst;
using Xunit;
using Erkenning = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Erkenning;
using IpdcProduct = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.IpdcProduct;

[Collection(nameof(SchorsErkenningCollection))]
public class Returns_Detail_With_Geregistreerde_Erkenning : IAsyncLifetime
{
    private readonly SchorsErkenningContext _context;

    public Returns_Detail_With_Geregistreerde_Erkenning(SchorsErkenningContext context)
    {
        _context = context;
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
                        _context.VCode.Value,
                        _context.Scenario.ErkenningWerdGeregistreerd.ErkenningId.ToString()),
                    type = JsonLdType.Erkenning.Type,
                    ErkenningId = _context.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                    GeregistreerdDoor = new GegevensInitiatorErkenning
                    {
                        OvoCode = _context.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
                        Naam = _context.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.Naam
                    },
                    IpdcProduct = new IpdcProduct
                    {
                        Nummer = _context.Scenario.ErkenningWerdGeregistreerd.IpdcProduct.Nummer,
                        Naam = _context.Scenario.ErkenningWerdGeregistreerd.IpdcProduct.Naam,
                    },
                    Startdatum = _context.Scenario.ErkenningWerdGeregistreerd.Startdatum?.ToString(WellknownFormats.DateOnly),
                    Einddatum = _context.Scenario.ErkenningWerdGeregistreerd.Einddatum?.ToString(WellknownFormats.DateOnly),
                    Hernieuwingsdatum = _context.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum?.ToString(WellknownFormats.DateOnly),
                    HernieuwingsUrl = _context.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                    RedenSchorsing = _context.CommandRequest.RedenSchorsing,
                    Status = ErkenningStatus.Geschorst,
                },
            ]);
    }

    public PubliekVerenigingDetailResponse Response { get; set; }

    public async ValueTask InitializeAsync()
    {
        Response = await _context.ApiSetup.PublicApiHost.GetPubliekDetail(_context.VCode);
    }

    public async ValueTask DisposeAsync()
    {
    }
}
