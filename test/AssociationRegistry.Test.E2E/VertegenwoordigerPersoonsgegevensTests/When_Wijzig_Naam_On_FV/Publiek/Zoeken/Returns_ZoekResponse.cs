namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Wijzig_Naam_On_FV.Publiek.Zoeken;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using Xunit;

[Collection(nameof(WijzigNaamOnFVTestCollection))]
public class Returns_SearchVerenigingenResponse
    : End2EndTest<SearchVerenigingenResponse>
{
    private readonly WijzigNaamOnFVTestContext _testContext;

    public Returns_SearchVerenigingenResponse(WijzigNaamOnFVTestContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.PublicApiHost.GetPubliekZoeken($"vCode:{_testContext.VCode}", _testContext.CommandResult.Sequence);

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Verenigingen.Single().Naam.Should().Be(_testContext.CommandRequest.Naam);
}
