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
public class Given_Sorting_By_Single_Field : End2EndTest<SearchContext, NullRequest, SearchVerenigingenResponse>
{
    private readonly SearchContext _testContext;

    public Given_Sorting_By_Single_Field(SearchContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Theory]
    [InlineData("naam")]
    [InlineData("korteNaam")]
    [InlineData("vCode")]
    public async Task Then_it_sorts_descending_V2(string field)
    {
        var result = await _testContext.ApiSetup.PublicApiHost.GetPubliekZoekenWithHeader(_testContext.ApiSetup.SuperAdminHttpClient,
                                                                                $"*&sort=-{field}");

        var verenigingen = result.Verenigingen;

        var fields = verenigingen
                    .Select(x => new
                     {
                         Field = GetPropertyValue(x, field)
                     }).Select(x => x.Field).ToList();

        fields.Should().BeInDescendingOrder();
    }

    [Theory]
    [InlineData("naam")]
    [InlineData("korteNaam")]
    [InlineData("vCode")]
    public async Task? Then_it_sorts_ascending_V2(string field)
    {
        var result = await _testContext.ApiSetup.PublicApiHost.GetPubliekZoekenWithHeader(_testContext.ApiSetup.SuperAdminHttpClient,
                                                                                $"*&sort={field}");

        var verenigingen = result.Verenigingen;

        var fields = verenigingen
                   .Select(x => new
                    {
                        Field = GetPropertyValue(x, field)
                    }).Select(x => x.Field).ToList();

        fields.Should().BeInAscendingOrder();
    }

    private static object GetPropertyValue(object obj, string propertyName)
    {
        var propInfo = obj.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        return propInfo?.GetValue(obj, null);
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekZoeken("*&sort=verenigingstype.code");
}
