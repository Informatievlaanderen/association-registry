namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo.Beheer.Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(WijzigBasisgegevensKbocollection))]
public class Returns_Historiek : End2EndTest<HistoriekResponse>
{
    private readonly WijzigBasisgegevensKboContext _testContext;

    public override HistoriekResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient, _testContext.VCode,
                                                 new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence))
                .GetAwaiter().GetResult();

    public Returns_Historiek(WijzigBasisgegevensKboContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

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
        var verenigingWerdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd));

        verenigingWerdGeregistreerd.ShouldCompare(
            HistoriekGebeurtenisMapper.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(_testContext.RegistratieData),
            compareConfig: HistoriekComparisonConfig.Instance);

        var naamWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(NaamWerdGewijzigd));

        var korteBeschrijvingWerdGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(KorteBeschrijvingWerdGewijzigd));

        korteBeschrijvingWerdGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.KorteBeschrijvingWerdGewijzigd(_testContext.VCode, _testContext.CommandRequest.KorteBeschrijving),
            compareConfig: HistoriekComparisonConfig.Instance);

        var doelgroepWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(DoelgroepWerdGewijzigd));

        doelgroepWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.DoelgroepWerdGewijzigd(_testContext.CommandRequest.Doelgroep),
                                             compareConfig: HistoriekComparisonConfig.Instance);

        var hoofdactiviteitenVerenigingsloketWerdenGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd));

        hoofdactiviteitenVerenigingsloketWerdenGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
                _testContext.CommandRequest.HoofdactiviteitenVerenigingsloket),
            compareConfig: HistoriekComparisonConfig.Instance);

        var werkingsgebiedenWerdenGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(WerkingsgebiedenWerdenGewijzigd));

        werkingsgebiedenWerdenGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.WerkingsgebiedenWerdenGewijzigd(_testContext.VCode, _testContext.CommandRequest.Werkingsgebieden),
            compareConfig: HistoriekComparisonConfig.Instance);

        var roepnaamWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(RoepnaamWerdGewijzigd));

        roepnaamWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.RoepnaamWerdGewijzigd(_testContext.CommandRequest.Roepnaam),
                                            compareConfig: HistoriekComparisonConfig.Instance);
    }
}
