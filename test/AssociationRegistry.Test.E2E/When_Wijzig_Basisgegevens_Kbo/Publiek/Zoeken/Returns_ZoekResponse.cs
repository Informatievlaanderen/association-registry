namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe.Zoeken;

using Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using JsonLdContext;
using Public.Api.Verenigingen.Search.ResponseModels;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using Vereniging;
using KellermanSoftware.CompareNetObjects;
using When_Wijzig_Basisgegevens_Kbo;
using When_Wijzig_Basisgegevens;
using Xunit;
using Vereniging = Public.Api.Verenigingen.Search.ResponseModels.Vereniging;

[Collection(nameof(WijzigBasisgegevensKbocollection))]
public class Returns_SearchVerenigingenResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly WijzigBasisgegevensKboContext _context;

    public Returns_SearchVerenigingenResponse(WijzigBasisgegevensKboContext context)
        : base(context.ApiSetup)
    {
        _context = context;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithVerenigingMetRechtspersoonlijkheid()
        => Response.Verenigingen.Single().ShouldCompare(new Vereniging
        {
            type = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_context.VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = _context.VCode,
            KorteBeschrijving = _context.CommandRequest.KorteBeschrijving,
            KorteNaam = _context.RegistratieData.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.VZW.Code,
                Naam = Verenigingstype.VZW.Naam,
            },
            Naam = _context.RegistratieData.Naam,
            Roepnaam = _context.CommandRequest.Roepnaam,
            HoofdactiviteitenVerenigingsloket = PubliekZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(_context.CommandRequest.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = PubliekZoekResponseMapper.MapWerkingsgebieden(_context.CommandRequest.Werkingsgebieden),
            Locaties = [],
            Lidmaatschappen = [],
            Relaties = [],
            Sleutels = PubliekZoekResponseMapper.MapSleutels(_context.VCode, _context.RegistratieData.KboNummer),
            Links = new VerenigingLinks()
            {
                Detail = new Uri($"{_context.PublicApiAppSettings.BaseUrl}/v1/verenigingen/{_context.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetPubliekZoeken($"vCode:{_context.VCode}");
}
