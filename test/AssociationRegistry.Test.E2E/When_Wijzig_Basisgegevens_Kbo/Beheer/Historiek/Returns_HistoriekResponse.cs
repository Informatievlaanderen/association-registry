namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo.Beheer.Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(WellKnownCollections.WijzigBasisgegevensKbo)]
public class Returns_Historiek : End2EndTest<WijzigBasisgegevensKboTestContext, WijzigBasisgegevensRequest, HistoriekResponse>
{
    public override Func<IApiSetup, HistoriekResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerHistoriek(TestContext.VCode);

    public Returns_Historiek(WijzigBasisgegevensKboTestContext testContext) : base(testContext)
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
        var verenigingWerdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd));

        verenigingWerdGeregistreerd.ShouldCompare(
            HistoriekGebeurtenisMapper.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(TestContext.RegistratieData),
            compareConfig: HistoriekComparisonConfig.Instance);

        var naamWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(NaamWerdGewijzigd));

        var korteBeschrijvingWerdGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(KorteBeschrijvingWerdGewijzigd));

        korteBeschrijvingWerdGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.KorteBeschrijvingWerdGewijzigd(TestContext.VCode, TestContext.Request.KorteBeschrijving),
            compareConfig: HistoriekComparisonConfig.Instance);

        var doelgroepWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(DoelgroepWerdGewijzigd));

        doelgroepWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.DoelgroepWerdGewijzigd(TestContext.Request.Doelgroep),
                                             compareConfig: HistoriekComparisonConfig.Instance);

        var hoofdactiviteitenVerenigingsloketWerdenGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd));

        hoofdactiviteitenVerenigingsloketWerdenGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
                TestContext.Request.HoofdactiviteitenVerenigingsloket),
            compareConfig: HistoriekComparisonConfig.Instance);

        var werkingsgebiedenWerdenGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(WerkingsgebiedenWerdenGewijzigd));

        werkingsgebiedenWerdenGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.WerkingsgebiedenWerdenGewijzigd(TestContext.VCode, TestContext.Request.Werkingsgebieden),
            compareConfig: HistoriekComparisonConfig.Instance);

        var roepnaamWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(RoepnaamWerdGewijzigd));

        roepnaamWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.RoepnaamWerdGewijzigd(TestContext.Request.Roepnaam),
                                            compareConfig: HistoriekComparisonConfig.Instance);
    }
}
