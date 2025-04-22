namespace AssociationRegistry.Test.E2E.When_Searching.Publiek.Zoeken.Sorting;

using Public.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Requests;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Given_Sorting_By_VerenigingsType : End2EndTest<SearchContext, NullRequest, SearchVerenigingenResponse>
{
    private readonly SearchContext _testContext;

    public Given_Sorting_By_VerenigingsType(SearchContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Fact]
    public void Then_it_sorts_by_Verenigingstype_then_by_vCode_descending()
    {
        var verenigingen = Response.Verenigingen;

        var groups = verenigingen.Select(x => new { x.VCode, x.Verenigingstype.Code })
                                 .GroupBy(x => x.Code, x => x.VCode)
                                 .ToDictionary(x => x.Key, x => x.ToList());

        groups.Keys.Should().BeInAscendingOrder();

        foreach (var group in groups)
        {
            group.Value.Should().BeInDescendingOrder();
        }
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekZoekenWithHeader(setup.SuperAdminHttpClient,"*&sort=verenigingstype.code").GetAwaiter().GetResult();
}
