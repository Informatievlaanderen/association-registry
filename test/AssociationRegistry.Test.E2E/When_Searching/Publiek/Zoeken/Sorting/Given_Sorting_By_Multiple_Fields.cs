namespace AssociationRegistry.Test.E2E.When_Searching.Publiek.Zoeken.Sorting;

using Public.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Requests;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using System.Reflection;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Given_Sorting_By_Multiple_Fields : End2EndTest<SearchContext, NullRequest, SearchVerenigingenResponse>
{
    private readonly SearchContext _testContext;

    public Given_Sorting_By_Multiple_Fields(SearchContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Theory]
    [InlineData("verenigingstype.code", "naam")]
    [InlineData("verenigingstype.code", "korteNaam")]
    [InlineData("verenigingstype.code", "vCode")]
    public async Task Then_it_sorts_by_Verenigingstype_then_by_vCode_descending_V2(string ascendingField, string descendingField)
    {
        var result = await _testContext.ApiSetup.PublicApiHost.GetPubliekZoekenWithHeader(_testContext.ApiSetup.SuperAdminHttpClient,
                                                                                $"*&sort={ascendingField},-{descendingField}");

        var verenigingen = result.Verenigingen;

        var groups = verenigingen
                    .Select(x => new
                     {
                         Code = x.Verenigingstype.Code,
                         DescField = GetPropertyValue(x, descendingField)
                     })
                    .GroupBy(x => x.Code, x => x.DescField)
                    .ToDictionary(x => x.Key, x => x.ToList());

        groups.Keys.Should().BeInAscendingOrder();

        foreach (var group in groups)
        {
            group.Value.Should().BeInDescendingOrder();
        }
    }

    [Theory]
    [InlineData("verenigingstype.code", "naam")]
    [InlineData("verenigingstype.code", "korteNaam")]
    [InlineData("verenigingstype.code", "vCode")]
    public async Task? Then_it_sorts_descending_then_ascending_V2(string descendingField, string ascendingField)
    {
        var result = await _testContext.ApiSetup.PublicApiHost.GetPubliekZoekenWithHeader(_testContext.ApiSetup.SuperAdminHttpClient,
                                                                                $"*&sort=-{descendingField},{ascendingField}");

        var verenigingen = result.Verenigingen;

        var groups = verenigingen
                    .Select(x => new
                     {
                         Code = x.Verenigingstype.Code,
                         DescField = GetPropertyValue(x, descendingField)
                     })
                    .GroupBy(x => x.Code, x => x.DescField)
                    .ToDictionary(x => x.Key, x => x.ToList());

        groups.Keys.Should().BeInDescendingOrder();

        foreach (var group in groups)
        {
            group.Value.Should().BeInAscendingOrder();
        }
    }

    private static object GetPropertyValue(object obj, string propertyName)
    {
        var propInfo = obj.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        return propInfo?.GetValue(obj, null);
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekZoeken("*&sort=verenigingstype.code");
}
