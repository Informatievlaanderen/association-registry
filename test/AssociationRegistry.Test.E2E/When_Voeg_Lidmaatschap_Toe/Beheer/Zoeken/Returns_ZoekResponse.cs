namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe.Beheer.Zoeken;

using AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VoegLidmaatschapToeCollection))]
public class Returns_SearchVerenigingenResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly VoegLidmaatschapToeContext _testContext;
    private readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;

    public Returns_SearchVerenigingenResponse(VoegLidmaatschapToeContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        FeitelijkeVerenigingWerdGeregistreerd = testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd;
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
    {
        Response.Verenigingen.Single().ShouldCompare(new Vereniging
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
            HoofdactiviteitenVerenigingsloket = BeheerZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = [],
            Locaties = BeheerZoekResponseMapper.MapLocaties(FeitelijkeVerenigingWerdGeregistreerd.Locaties, _testContext.VCode),
            Sleutels = BeheerZoekResponseMapper.MapSleutels(_testContext.VCode),
            Lidmaatschappen = BeheerZoekResponseMapper.MapLidmaatschappen(_testContext.CommandRequest, _testContext.VCode),
            Links = new VerenigingLinks()
            {
                Detail = new Uri($"{_testContext.AdminApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);
    }
}
