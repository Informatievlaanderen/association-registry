namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.HistoriekAuthentiekeVereniging;

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
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Historiek : End2EndTest<MarkeerAlsDubbelVanContext, MarkeerAlsDubbelVanRequest, HistoriekResponse>
{
    private readonly ITestOutputHelper _helper;

    public override Func<IApiSetup, HistoriekResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerHistoriek(TestContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);

    public Returns_Historiek(MarkeerAlsDubbelVanContext testContext, ITestOutputHelper helper)
    {
        _helper = helper;
    }

    [Fact]
    public async ValueTask With_VCode()
    {
        var tryCounter = 0;

        while (tryCounter < 20)
        {
            ++tryCounter;
            await Task.Delay(500);

            _helper.WriteLine($"Looking for CorresponderendeVCodes (try {tryCounter})...");
            if (Response.VCode is not null)
            {
                _helper.WriteLine("Found it!");
                break;
            }

            _helper.WriteLine("Did not find any CorresponderendeVCodes.");
        }

        Response.VCode.ShouldCompare(TestContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public async ValueTask With_All_Gebeurtenissen()
    {
        var gebeurtenis =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VerenigingAanvaarddeDubbeleVereniging));

        var tryCounter = 0;

        while (tryCounter < 20)
        {
            ++tryCounter;
            await Task.Delay(500);

            _helper.WriteLine($"Looking for CorresponderendeVCodes (try {tryCounter})...");
            if (gebeurtenis is not null)
            {
                _helper.WriteLine("Found it!");
                break;
            }

            _helper.WriteLine("Did not find any CorresponderendeVCodes.");
        }

        gebeurtenis.ShouldCompare(HistoriekGebeurtenisMapper.VerenigingAanvaarddeDubbeleVereniging(TestContext.Request, TestContext.VCode),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
