namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Wijzig_Naam_On_KBO.Publiek.Zoeken;

using AssociationRegistry.Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(WijzigRoepnaamOnKBOTestCollection))]
public class Returns_SearchVerenigingenResponse
    : End2EndTest<SearchVerenigingenResponse>
{
    private readonly WijzigRoepnaamOnKBOTestContext _testContext;

    public Returns_SearchVerenigingenResponse(WijzigRoepnaamOnKBOTestContext testContext) : base(testContext.ApiSetup)
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
        => Response.Verenigingen.Single().Roepnaam.Should().Be(_testContext.CommandRequest.Roepnaam);
}
