namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.Zoeken;

using Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using Admin.Api.DecentraalBeheer.Verenigingen.Search.ResponseModels;
using Events;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<MarkeerAlsDubbelVanContext, MarkeerAlsDubbelVanRequest, SearchVerenigingenResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;
    private readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;

    public Returns_SearchVerenigingenResponse(MarkeerAlsDubbelVanContext testContext) : base(testContext)
    {
        _testContext = testContext;
        FeitelijkeVerenigingWerdGeregistreerd = testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async Task With_Verenigingen_Empty()
    {
        Response.Verenigingen.Should().BeEmpty();
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerZoeken($"vCode:{_testContext.VCode}");
}
