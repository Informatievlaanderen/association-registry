namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap.Zoeken;

using Admin.Api.Verenigingen.Search.ResponseModels;
using Events;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Framework.Comparison;
using Framework.Mappers;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Xunit;
using Vereniging = Admin.Api.Verenigingen.Search.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Admin.Api.Verenigingen.Search.ResponseModels.Verenigingstype;

[Collection(nameof(VerwijderLidmaatschapCollection))]
public class Returns_SearchVerenigingenResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly VerwijderLidmaatschapContext _testContext;
    private readonly FeitelijkeVerenigingWerdGeregistreerd _feitelijkeVerenigingWerdGeregistreerd;

    public Returns_SearchVerenigingenResponse(VerwijderLidmaatschapContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _feitelijkeVerenigingWerdGeregistreerd = testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeregistreerd;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
    {
        Response.Verenigingen.Single().ShouldCompare(new Vereniging()
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = _feitelijkeVerenigingWerdGeregistreerd.Doelgroep.Minimumleeftijd,
                Maximumleeftijd = _feitelijkeVerenigingWerdGeregistreerd.Doelgroep.Maximumleeftijd,
            },
            VCode = _testContext.VCode,
            KorteNaam = _feitelijkeVerenigingWerdGeregistreerd.KorteNaam,
            Verenigingstype = new Verenigingstype
            {
                Code = AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = _feitelijkeVerenigingWerdGeregistreerd.Naam,
            Startdatum = _feitelijkeVerenigingWerdGeregistreerd.Startdatum.FormatAsBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            HoofdactiviteitenVerenigingsloket = BeheerZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(_feitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = [],
            Locaties = BeheerZoekResponseMapper.MapLocaties(_feitelijkeVerenigingWerdGeregistreerd.Locaties, _testContext.VCode),
            Sleutels = BeheerZoekResponseMapper.MapSleutels(_testContext.VCode),
            Lidmaatschappen = [],
            Links = new VerenigingLinks
            {
                Detail = new Uri($"{_testContext.AdminApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);
    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerZoeken($"vCode:{_testContext.VCode}");
}
