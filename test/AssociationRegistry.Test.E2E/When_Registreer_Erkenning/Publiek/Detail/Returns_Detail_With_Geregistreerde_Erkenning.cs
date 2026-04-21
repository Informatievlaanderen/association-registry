namespace AssociationRegistry.Test.E2E.When_Registreer_Erkenning.Publiek.Detail;

using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Framework.AlbaHost;
using KellermanSoftware.CompareNetObjects;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Xunit;
using Erkenning = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Erkenning;
using IpdcProduct = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.IpdcProduct;

[Collection(nameof(RegistreerErkenningCollection))]
public class Returns_Detail_With_Geregistreerde_Erkenning : IAsyncLifetime
{
    private readonly RegistreerErkenningContext _context;

    public Returns_Detail_With_Geregistreerde_Erkenning(RegistreerErkenningContext context)
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
                    id = JsonLdType.Erkenning.CreateWithIdValues(_context.VCode.Value, "1"),
                    type = JsonLdType.Erkenning.Type,
                    ErkenningId = 1,
                    GeregistreerdDoor = new GegevensInitiatorErkenning { OvoCode = "OVO000001", Naam = string.Empty },
                    IpdcProduct = new IpdcProduct
                    {
                        Nummer = _context.CommandRequest.Erkenning.IpdcProductNummer,
                        Naam = "Gemeentelijke premie voor inbraakpreventie", // See wiremock
                    },
                    Startdatum = _context.CommandRequest.Erkenning.Startdatum,
                    Einddatum = _context.CommandRequest.Erkenning.Einddatum,
                    Hernieuwingsdatum = _context.CommandRequest.Erkenning.Hernieuwingsdatum,
                    HernieuwingsUrl = _context.CommandRequest.Erkenning.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = ErkenningStatus.Bepaal(
                        ErkenningsPeriode.Create(
                            _context.CommandRequest.Erkenning.Startdatum,
                            _context.CommandRequest.Erkenning.Einddatum
                        ),
                        DateOnly.FromDateTime(DateTime.Now)
                    ),
                },
            ]);
    }

    public PubliekVerenigingDetailResponse Response { get; set; }

    public async ValueTask InitializeAsync()
    {
        Response = await _context.ApiSetup.PublicApiHost.GetPubliekDetail(_context.VCode);
    }

    public async ValueTask DisposeAsync() { }
}
