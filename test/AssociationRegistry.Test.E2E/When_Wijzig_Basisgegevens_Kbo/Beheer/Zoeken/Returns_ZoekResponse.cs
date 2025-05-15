namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo.Beheer.Zoeken;

using Admin.Api.Verenigingen.Search.ResponseModels;
using Formats;
using JsonLdContext;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Marten;
using NodaTime;

using Xunit;
using Vereniging = Admin.Api.Verenigingen.Search.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Admin.Api.Verenigingen.Search.ResponseModels.Verenigingstype;

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
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
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
                Minimumleeftijd = _testContext.CommandRequest.Doelgroep.Minimumleeftijd.Value,
                Maximumleeftijd = _testContext.CommandRequest.Doelgroep.Maximumleeftijd.Value,
            },
            VCode = _testContext.VCode,
            KorteNaam = _testContext.RegistratieData.KorteNaam,
            Verenigingstype = new Verenigingstype
            {
                Code = AssociationRegistry.Vereniging.Verenigingstype.VZW.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingstype.VZW.Naam,
            },
            Naam = _testContext.RegistratieData.Naam,
            Roepnaam = _testContext.CommandRequest.Roepnaam,
            Startdatum = Instant.FromDateTimeOffset(
                new DateTimeOffset(_testContext.RegistratieData.Startdatum.Value.ToDateTime(new TimeOnly(12, 0, 0)))
            ).FormatAsBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            HoofdactiviteitenVerenigingsloket = BeheerZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(_testContext.CommandRequest.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = BeheerZoekResponseMapper.MapWerkingsgebieden(_testContext.CommandRequest.Werkingsgebieden),
            Locaties = [],
            Sleutels = BeheerZoekResponseMapper.MapSleutels(_testContext.VCode, _testContext.RegistratieData.KboNummer),
            Lidmaatschappen = [],
            Links = new VerenigingLinks()
            {
                Detail = new Uri($"{_testContext.AdminApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient, $"vCode:{_testContext.VCode}", setup.AdminApiHost.DocumentStore(), headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();
}
