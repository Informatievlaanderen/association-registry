namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging.Beheer_Historiek;

using Alba;
using AssociationRegistry.Admin.Api.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(RegistreerFeitelijkeVerenigingContext<AdminApiSetup>))]
public class Returns_Historiek(RegistreerFeitelijkeVerenigingContext<AdminApiSetup> context)
    : End2EndTest<RegistreerFeitelijkeVerenigingContext<AdminApiSetup>, RegistreerFeitelijkeVerenigingRequest, HistoriekResponse>(context)
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
