namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Beheer.ZoekenAuthentiekeVereniging;

using Admin.Api.Verenigingen.Search.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using JasperFx.Core;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Xunit;

[Collection(nameof(CorrigeerMarkeringAlsDubbelVanCollection))]
public class Returns_SearchVerenigingenResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _testContext;

    public Returns_SearchVerenigingenResponse(CorrigeerMarkeringAlsDubbelVanContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
    {
        Task.Delay(5.Seconds()).GetAwaiter().GetResult();

        return setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient, $"vCode:{_testContext.Scenario.AuthentiekeVereniging.VCode}",
                                                  setup.AdminApiHost.DocumentStore(),headers: new RequestParameters().WithExpectedSequence(
                                                      _testContext.AanvaarddeCorrectieDubbeleVereniging!.Sequence)).GetAwaiter()
                    .GetResult();
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
