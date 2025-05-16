namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Beheer.HistoriekAuthentiekeVereniging;

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

[Collection(nameof(CorrigeerMarkeringAlsDubbelVanCollection))]
public class Returns_Detail_With_Dubbel_Van : End2EndTest<HistoriekResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _testContext;
    private readonly ITestOutputHelper _helper;

    public Returns_Detail_With_Dubbel_Van(CorrigeerMarkeringAlsDubbelVanContext testContext, ITestOutputHelper helper) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _helper = helper;
    }

    public override HistoriekResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerHistoriek(_testContext.Scenario.AuthentiekeVereniging.VCode);

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

        Response.VCode.ShouldCompare(_testContext.Scenario.AuthentiekeVereniging.VCode);
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
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VerenigingAanvaarddeCorrectieDubbeleVereniging));

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

        gebeurtenis.ShouldCompare(HistoriekGebeurtenisMapper.AanvaardingDubbeleVerenigingWerdGecorrigeerd(_testContext.Scenario.VerenigingAanvaarddeDubbeleVereniging),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
