namespace AssociationRegistry.Test.E2E.V2.When_Wijzig_Basisgegevens.Beheer_Historiek;

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
        /*HandleNaam(vereniging, message.Command.Naam);
        HandleKorteNaam(vereniging, message.Command.KorteNaam);
        HandleKorteBeschrijving(vereniging, message.Command.KorteBeschrijving);
        HandleStartdatum(vereniging, message.Command.Startdatum, clock);
        WijzigHoofdactiviteitenVerenigingsloket(vereniging, message.Command.HoofdactiviteitenVerenigingsloket);
        HandleUitgeschrevenUitPubliekeDatastroom(vereniging, message.Command.IsUitgeschrevenUitPubliekeDatastroom);
        HandleDoelgroep(vereniging, message.Command.Doelgroep);*/
        var werdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(FeitelijkeVerenigingWerdGeregistreerd));

        werdGeregistreerd.ShouldCompare(HistoriekGebeurtenisMapper.FeitelijkeVerenigingWerdGeregistreerd(TestContext.RegistratieData),
                                        compareConfig: HistoriekComparisonConfig.Instance);

        // TODO: add werkingsgebiedwerdgewijzigd
        // var werkgebiedWerdGewijzigd =
        //     Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(WerkgebiedWerdGewijzigd));
        //
        // werdGeregistreerd.ShouldCompare(HistoriekGebeurtenisMapper.FeitelijkeVerenigingWerdGeregistreerd(TestContext.RegistratieData),
        //                                 compareConfig: HistoriekComparisonConfig.Instance);
    }
}
