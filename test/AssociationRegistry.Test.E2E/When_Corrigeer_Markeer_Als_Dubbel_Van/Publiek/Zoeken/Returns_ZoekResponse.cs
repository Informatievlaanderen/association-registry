namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Publiek.Zoeken;

using Public.Api.Verenigingen.Search.ResponseModels;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(CorrigeerMarkeringAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<SearchVerenigingenResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _testContext;

    public Returns_Vereniging(CorrigeerMarkeringAlsDubbelVanContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetPubliekZoeken($"vCode:{_testContext.VCode}", _testContext.CommandResult.Sequence);

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
}
