namespace AssociationRegistry.Test.E2E.When_Registreer_Vereniging.Beheer_Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Alba;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(RegistreerVerenigingContext<AdminApiSetup>))]
public class Returns_Historiek(RegistreerVerenigingContext<AdminApiSetup> context)
    : End2EndTest<RegistreerVerenigingContext<AdminApiSetup>, RegistreerFeitelijkeVerenigingRequest, HistoriekResponse>(context)
{
    protected override Func<IAlbaHost, HistoriekResponse> GetResponse => adminApi => adminApi.GetHistoriek(VCode);

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_All_Gebeurtenissen()
    {
        var werdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(FeitelijkeVerenigingWerdGeregistreerd));

        werdGeregistreerd.ShouldCompare(HistoriekGebeurtenisMapper.FeitelijkeVerenigingWerdGeregistreerd(Request, VCode),
                                        compareConfig: HistoriekComparisonConfig.Instance);
        // Response.Gebeurtenissen.ShouldCompare([
        //     HistoriekGebeurtenisMapper.FeitelijkeVerenigingWerdGeregistreerd(Request, VCode),
        //     HistoriekGebeurtenisMapper.AdresWerdOvergenomen(VCode),
        //     HistoriekGebeurtenisMapper.AdresNietUniekInAR(VCode),
        //     HistoriekGebeurtenisMapper.AdresKonNietOvergenomenWorden(VCode),
        // ], compareConfig: HistoriekComparisonConfig.Instance);
    }
}
