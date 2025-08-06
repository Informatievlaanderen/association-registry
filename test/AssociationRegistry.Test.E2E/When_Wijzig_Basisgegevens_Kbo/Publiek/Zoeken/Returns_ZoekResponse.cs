namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo.Publiek.Zoeken;

using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using KellermanSoftware.CompareNetObjects;
using Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using Xunit;
using Vereniging = Public.Api.WebApi.Verenigingen.Search.ResponseModels.Vereniging;

[Collection(nameof(WijzigBasisgegevensKbocollection))]
public class Returns_SearchVerenigingenResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly WijzigBasisgegevensKboContext _testContext;

    public Returns_SearchVerenigingenResponse(WijzigBasisgegevensKboContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
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
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = _testContext.VCode,
            KorteBeschrijving = _testContext.CommandRequest.KorteBeschrijving,
            KorteNaam = _testContext.RegistratieData.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.VZW.Code,
                Naam = Verenigingstype.VZW.Naam,
            },
            Naam = _testContext.RegistratieData.Naam,
            Roepnaam = _testContext.CommandRequest.Roepnaam,
            HoofdactiviteitenVerenigingsloket = PubliekZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(_testContext.CommandRequest.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = PubliekZoekResponseMapper.MapWerkingsgebieden(_testContext.CommandRequest.Werkingsgebieden),
            Locaties = [],
            Lidmaatschappen = [],
            Relaties = [],
            Sleutels = PubliekZoekResponseMapper.MapSleutels(_testContext.VCode, _testContext.RegistratieData.KboNummer),
            Links = new VerenigingLinks()
            {
                Detail = new Uri($"{_testContext.PublicApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetPubliekZoeken($"vCode:{_testContext.VCode}", _testContext.CommandResult.Sequence);
}
