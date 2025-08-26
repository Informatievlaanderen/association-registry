namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Beheer.ZoekenAuthentiekeVereniging;

using Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using JasperFx.Core;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(CorrigeerMarkeringAlsDubbelVanCollection))]
public class Returns_SearchVerenigingenResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _testContext;
    private readonly ITestOutputHelper _helper;

    public Returns_SearchVerenigingenResponse(CorrigeerMarkeringAlsDubbelVanContext testContext, ITestOutputHelper helper) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _helper = helper;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup)
    {
        await Task.Delay(5.Seconds());

        return await setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient, $"vCode:{_testContext.Scenario.AuthentiekeVereniging.VCode}",
                                                        setup.AdminApiHost.DocumentStore(),
                                                        
                                                        headers: new RequestParameters().WithExpectedSequence(
                                                            _testContext.AanvaarddeCorrectieDubbeleVereniging!.Sequence),
                                                        testOutputHelper: _helper);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public void WithFeitelijkeVereniging()
    {
        Response.Verenigingen.Single(x => x.VCode == _testContext.Scenario.AuthentiekeVereniging.VCode)
                .CorresponderendeVCodes.Should().BeEmpty();
    }
}
