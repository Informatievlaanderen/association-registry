namespace AssociationRegistry.Test.E2E.When_Searching.Beheer.Zoeken.Sorting;

using Admin.Api.Verenigingen.Search.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Scenarios.Requests;
using System.Reflection;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Given_Sorting_By_Single_Field : End2EndTest<SearchContext, NullRequest, SearchVerenigingenResponse>
{
    private readonly SearchContext _testContext;

    public Given_Sorting_By_Single_Field(SearchContext testContext)
    {
        TestContext = _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Theory]
    [InlineData("naam")]
    [InlineData("korteNaam")]
    [InlineData("vCode")]
    public async Task Then_it_sorts_descending_V2(string field)
    {
        var result = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoekenV2(_testContext.ApiSetup.SuperAdminHttpClient,
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
        var result = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoekenV2(_testContext.ApiSetup.SuperAdminHttpClient,
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
        => setup => setup.AdminApiHost.GetBeheerZoeken("*&sort=verenigingstype.code");
}
