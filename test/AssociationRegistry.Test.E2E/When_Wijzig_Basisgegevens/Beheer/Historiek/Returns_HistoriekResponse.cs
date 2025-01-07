namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens.Beheer.Historiek;

using Admin.Api.DecentraalBeheer.Verenigingen.Historiek.ResponseModels;
using Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Historiek : End2EndTest<WijzigBasisgegevensTestContext, WijzigBasisgegevensRequest, HistoriekResponse>
{
    public override Func<IApiSetup, HistoriekResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerHistoriek(TestContext.VCode);

    public Returns_Historiek(WijzigBasisgegevensTestContext testContext) : base(testContext)
    {
    }

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(TestContext.VCode);
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

        feitelijkeVerenigingWerdGeregistreerd.ShouldCompare(HistoriekGebeurtenisMapper.FeitelijkeVerenigingWerdGeregistreerd(TestContext.RegistratieData),
                                        compareConfig: HistoriekComparisonConfig.Instance);

        var naamWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(NaamWerdGewijzigd));
        naamWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.NaamWerdGewijzigd(TestContext.VCode, TestContext.Request.Naam),
                                        compareConfig: HistoriekComparisonConfig.Instance);

        var korteNaamWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(KorteNaamWerdGewijzigd));
        korteNaamWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.KorteNaamWerdGewijzigd(TestContext.VCode, TestContext.Request.KorteNaam),
                                             compareConfig: HistoriekComparisonConfig.Instance);

        var korteBeschrijvingWerdGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(KorteBeschrijvingWerdGewijzigd));
        korteBeschrijvingWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.KorteBeschrijvingWerdGewijzigd(TestContext.VCode, TestContext.Request.KorteBeschrijving),
                                                     compareConfig: HistoriekComparisonConfig.Instance);

        var startdatumWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(StartdatumWerdGewijzigd));
        startdatumWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.StartdatumWerdGewijzigd(TestContext.VCode, TestContext.Request.Startdatum.Value),
                                              compareConfig: HistoriekComparisonConfig.Instance);

        var hoofdactiviteitenVerenigingsloketWerdenGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd));
        hoofdactiviteitenVerenigingsloketWerdenGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.HoofdactiviteitenVerenigingsloketWerdenGewijzigd(TestContext.Request.HoofdactiviteitenVerenigingsloket),
                                                                       compareConfig: HistoriekComparisonConfig.Instance);

        var publiekeDatastroomWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(
            x => x.Gebeurtenis == nameof(VerenigingWerdIngeschrevenInPubliekeDatastroom) ||
                 x.Gebeurtenis == nameof(VerenigingWerdUitgeschrevenUitPubliekeDatastroom));
        publiekeDatastroomWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.IsUitgeschrevenUitPubliekeDatastroom(TestContext.Request.IsUitgeschrevenUitPubliekeDatastroom),
                                                      compareConfig: HistoriekComparisonConfig.Instance);

        var doelgroepWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(DoelgroepWerdGewijzigd));
        doelgroepWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.DoelgroepWerdGewijzigd(TestContext.Request.Doelgroep),
                                             compareConfig: HistoriekComparisonConfig.Instance);

        var werkingsgebiedenWerdenBepaald = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(WerkingsgebiedenWerdenBepaald));
        werkingsgebiedenWerdenBepaald.ShouldCompare(HistoriekGebeurtenisMapper.WerkingsgebiedenWerdenBepaald(TestContext.VCode, TestContext.Request.Werkingsgebieden),
                                             compareConfig: HistoriekComparisonConfig.Instance);
    }
}
