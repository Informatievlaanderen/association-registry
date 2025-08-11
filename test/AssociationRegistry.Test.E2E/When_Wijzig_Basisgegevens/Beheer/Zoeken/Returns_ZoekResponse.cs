namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens.Beheer.Zoeken;

using Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using Contracts.JsonLdContext;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Marten;
using NodaTime;
using Xunit;
using Vereniging = Admin.Api.WebApi.Verenigingen.Search.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Admin.Api.WebApi.Verenigingen.Search.ResponseModels.Verenigingstype;

[Collection(nameof(WijzigBasisGegevensCollection))]
public class Returns_SearchVerenigingenResponse

    : End2EndTest<SearchVerenigingenResponse>
{
    private readonly WijzigBasisgegevensContext _testContext;

    public Returns_SearchVerenigingenResponse(WijzigBasisgegevensContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient, $"vCode:{_testContext.VCode}", setup.AdminApiHost.DocumentStore(), headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();


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
                Minimumleeftijd = _testContext.CommandRequest.Doelgroep.Minimumleeftijd.Value,
                Maximumleeftijd = _testContext.CommandRequest.Doelgroep.Maximumleeftijd.Value,
            },
            VCode = _testContext.VCode,
            KorteNaam = _testContext.CommandRequest.KorteNaam,
            Verenigingstype = new Verenigingstype
            {
                Code = DecentraalBeheer.Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                Naam = DecentraalBeheer.Vereniging.Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = _testContext.CommandRequest.Naam,
            Startdatum = Instant.FromDateTimeOffset(
                new DateTimeOffset(_testContext.CommandRequest.Startdatum.Value.ToDateTime(new TimeOnly(12, 0, 0)))
            ).FormatAsBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            HoofdactiviteitenVerenigingsloket = BeheerZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(_testContext.CommandRequest.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = BeheerZoekResponseMapper.MapWerkingsgebieden(_testContext.CommandRequest.Werkingsgebieden),
            Locaties = BeheerZoekResponseMapper.MapLocaties(_testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties, _testContext.VCode),
            Sleutels = BeheerZoekResponseMapper.MapSleutels(_testContext.VCode),
            Lidmaatschappen = [],
            Links = new VerenigingLinks()
            {
                Detail = new Uri($"{_testContext.AdminApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);

}
