namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens.Beheer.Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(WijzigBasisGegevensCollection))]
public class Returns_Historiek : End2EndTest<HistoriekResponse>
{
    private readonly WijzigBasisgegevensContext _testContext;

    public Returns_Historiek(WijzigBasisgegevensContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override HistoriekResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient, _testContext.VCode, headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(_testContext.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_All_Gebeurtenissen()
    {
        var feitelijkeVerenigingWerdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(FeitelijkeVerenigingWerdGeregistreerd));

        feitelijkeVerenigingWerdGeregistreerd.ShouldCompare(
            HistoriekGebeurtenisMapper.FeitelijkeVerenigingWerdGeregistreerd(_testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd),
            compareConfig: HistoriekComparisonConfig.Instance);

        var naamWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(NaamWerdGewijzigd));

        naamWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.NaamWerdGewijzigd(_testContext.VCode, _testContext.CommandRequest.Naam),
                                        compareConfig: HistoriekComparisonConfig.Instance);

        var korteNaamWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(KorteNaamWerdGewijzigd));

        korteNaamWerdGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.KorteNaamWerdGewijzigd(_testContext.VCode, _testContext.CommandRequest.KorteNaam),
            compareConfig: HistoriekComparisonConfig.Instance);

        var korteBeschrijvingWerdGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(KorteBeschrijvingWerdGewijzigd));

        korteBeschrijvingWerdGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.KorteBeschrijvingWerdGewijzigd(_testContext.VCode, _testContext.CommandRequest.KorteBeschrijving),
            compareConfig: HistoriekComparisonConfig.Instance);

        var startdatumWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(StartdatumWerdGewijzigd));

        startdatumWerdGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.StartdatumWerdGewijzigd(_testContext.VCode, _testContext.CommandRequest.Startdatum.Value),
            compareConfig: HistoriekComparisonConfig.Instance);

        var hoofdactiviteitenVerenigingsloketWerdenGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd));

        hoofdactiviteitenVerenigingsloketWerdenGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
                _testContext.CommandRequest.HoofdactiviteitenVerenigingsloket),
            compareConfig: HistoriekComparisonConfig.Instance);

        var publiekeDatastroomWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(
            x => x.Gebeurtenis == nameof(VerenigingWerdIngeschrevenInPubliekeDatastroom) ||
                 x.Gebeurtenis == nameof(VerenigingWerdUitgeschrevenUitPubliekeDatastroom));

        publiekeDatastroomWerdGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.IsUitgeschrevenUitPubliekeDatastroom(_testContext.CommandRequest.IsUitgeschrevenUitPubliekeDatastroom),
            compareConfig: HistoriekComparisonConfig.Instance);

        var doelgroepWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(DoelgroepWerdGewijzigd));

        doelgroepWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.DoelgroepWerdGewijzigd(_testContext.CommandRequest.Doelgroep),
                                             compareConfig: HistoriekComparisonConfig.Instance);

        var werkingsgebiedenWerdenBepaald =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(WerkingsgebiedenWerdenBepaald));

        werkingsgebiedenWerdenBepaald.ShouldCompare(
            HistoriekGebeurtenisMapper.WerkingsgebiedenWerdenBepaald(_testContext.VCode, _testContext.CommandRequest.Werkingsgebieden),
            compareConfig: HistoriekComparisonConfig.Instance);
    }
}
