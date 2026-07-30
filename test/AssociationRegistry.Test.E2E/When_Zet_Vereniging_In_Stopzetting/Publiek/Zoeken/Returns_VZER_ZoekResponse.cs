namespace AssociationRegistry.Test.E2E.When_Zet_Vereniging_In_Stopzetting.Publiek.Zoeken;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using Xunit;

[Collection(nameof(ZetVerenigingInStopzettingCollection))]
public class Returns_Vereniging : End2EndTest<SearchVerenigingenResponse>
{
    private readonly ZetVerenigingInStopzettingContext _testContext;
    private readonly ITestOutputHelper _testOutputHelper;

    public Returns_Vereniging(ZetVerenigingInStopzettingContext testContext, ITestOutputHelper testOutputHelper)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _testOutputHelper = testOutputHelper;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.PublicApiHost.GetPubliekZoeken($"vCode:{_testContext.VCode}", _testContext.CommandResult.Sequence);

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging() => Response.Verenigingen.Single().InStopzetting.Should().BeTrue();
}
