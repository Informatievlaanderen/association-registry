namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Beheer.HistoriekAuthentiekeVereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Scenarios.Requests;
using Xunit;
using Xunit.Abstractions;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Historiek : End2EndTest<CorrigeerMarkeringAlsDubbelVanContext, NullRequest, HistoriekResponse>
{
    private readonly ITestOutputHelper _helper;

    public override Func<IApiSetup, HistoriekResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerHistoriek(TestContext.Scenario.AuthentiekeVereniging.VCode);

    public Returns_Historiek(CorrigeerMarkeringAlsDubbelVanContext testContext, ITestOutputHelper helper) : base(testContext)
    {
        _helper = helper;
    }

    [Fact]
    public async Task With_VCode()
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

        Response.VCode.ShouldCompare(TestContext.Scenario.AuthentiekeVereniging.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public async Task With_All_Gebeurtenissen()
    {
        var gebeurtenis =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(AanvaardingDubbeleVerenigingWerdGecorrigeerd));

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

        gebeurtenis.ShouldCompare(HistoriekGebeurtenisMapper.AanvaardingDubbeleVerenigingWerdGecorrigeerd(TestContext.Scenario.VerenigingAanvaarddeDubbeleVereniging),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
