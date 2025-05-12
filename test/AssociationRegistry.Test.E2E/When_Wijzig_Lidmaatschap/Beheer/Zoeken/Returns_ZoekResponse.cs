namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap.Beheer.Zoeken;

using Admin.Api.Verenigingen.Search.ResponseModels;
using Events;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Scenarios.Givens.FeitelijkeVereniging;
using Xunit;
using DoelgroepResponse = Admin.Api.Verenigingen.Search.ResponseModels.DoelgroepResponse;
using Vereniging = Admin.Api.Verenigingen.Search.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Admin.Api.Verenigingen.Search.ResponseModels.Verenigingstype;

[Collection(nameof(WijzigLidmaatschapCollection))]
public class Returns_SearchVerenigingenResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly WijzigLidmaatschapContext _testContext;
    private readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    private readonly LidmaatschapWerdToegevoegdScenario _castedScenario;

    public Returns_SearchVerenigingenResponse(WijzigLidmaatschapContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _castedScenario = (LidmaatschapWerdToegevoegdScenario)testContext.Scenario;
        FeitelijkeVerenigingWerdGeregistreerd = _castedScenario.BaseScenario.FeitelijkeVerenigingWerdGeregistreerd;

    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerZoeken($"vCode:{_testContext.VCode}");

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Verenigingen.Single().ShouldCompare(new Vereniging
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = FeitelijkeVerenigingWerdGeregistreerd.Doelgroep.Minimumleeftijd,
                Maximumleeftijd = FeitelijkeVerenigingWerdGeregistreerd.Doelgroep.Maximumleeftijd,
            },
            VCode = _testContext.VCode,
            KorteNaam = FeitelijkeVerenigingWerdGeregistreerd.KorteNaam,
            Verenigingstype = new Verenigingstype
            {
                Code = AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = FeitelijkeVerenigingWerdGeregistreerd.Naam,
            Startdatum = FeitelijkeVerenigingWerdGeregistreerd.Startdatum.FormatAsBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            HoofdactiviteitenVerenigingsloket =
                BeheerZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(
                    FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = [],
            Locaties = BeheerZoekResponseMapper.MapLocaties(FeitelijkeVerenigingWerdGeregistreerd.Locaties, _testContext.VCode),
            Sleutels = BeheerZoekResponseMapper.MapSleutels(_testContext.VCode),
            Lidmaatschappen = BeheerZoekResponseMapper.MapLidmaatschappen(_testContext.CommandRequest, _testContext.VCode,
                                                                          _castedScenario.LidmaatschapWerdToegevoegd.Lidmaatschap
                                                                                      .AndereVereniging,
                                                                          _castedScenario.LidmaatschapWerdToegevoegd.Lidmaatschap
                                                                                      .LidmaatschapId),
            Links = new VerenigingLinks
            {
                Detail = new Uri($"{_testContext.AdminApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);
}
