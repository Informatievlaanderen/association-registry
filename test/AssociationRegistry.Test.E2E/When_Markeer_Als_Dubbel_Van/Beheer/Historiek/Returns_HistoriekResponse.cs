namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.Historiek;

using Admin.Api.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(MarkeerAlsDubbelVanContextCollection.Name)]
public class Returns_Historiek : End2EndTest<MarkeerAlsDubbelVanContext, MarkeerAlsDubbelVanRequest, HistoriekResponse>
{
    public override Func<IApiSetup, HistoriekResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerHistoriek(TestContext.VCode);

    public Returns_Historiek(MarkeerAlsDubbelVanContext testContext) : base(testContext)
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
        var gebeurtenis =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VerenigingWerdGemarkeerdAlsDubbelVan));

        gebeurtenis.ShouldCompare(HistoriekGebeurtenisMapper.VerenigingWerdGermarkeerdAlsDubbelVan(TestContext.Request, TestContext.VCode),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
