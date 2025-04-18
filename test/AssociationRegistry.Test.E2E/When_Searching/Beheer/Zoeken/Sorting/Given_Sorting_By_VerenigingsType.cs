namespace AssociationRegistry.Test.E2E.When_Searching.Beheer.Zoeken.Sorting;

using Admin.Api.Verenigingen.Search.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Scenarios.Requests;
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
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
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
        => setup => setup.AdminApiHost.GetBeheerZoeken("*&sort=verenigingstype.code");
}
