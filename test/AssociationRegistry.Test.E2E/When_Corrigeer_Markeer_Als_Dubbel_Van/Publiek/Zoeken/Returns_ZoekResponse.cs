namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Publiek.Zoeken;

using Public.Api.Verenigingen.Search.ResponseModels;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Scenarios.Requests;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<CorrigeerMarkeringAlsDubbelVanContext, NullRequest, SearchVerenigingenResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _testContext;

    public Returns_SearchVerenigingenResponse(CorrigeerMarkeringAlsDubbelVanContext testContext)
    {
        TestContext = _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask With_Verenigingen_Empty()
    {
        Response.Verenigingen.Should().NotBeEmpty();
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekZoeken($"vCode:{_testContext.VCode}");
}
