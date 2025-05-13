namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.Zoeken;

using Admin.Api.Verenigingen.Search.ResponseModels;
using Events;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<SearchVerenigingenResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; set; }

    public Returns_Vereniging(MarkeerAlsDubbelVanContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        FeitelijkeVerenigingWerdGeregistreerd = testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd;

    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerZoeken($"vCode:{_testContext.VCode}");

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask With_Verenigingen_Empty()
    {
        Response.Verenigingen.Should().BeEmpty();
    }
}
