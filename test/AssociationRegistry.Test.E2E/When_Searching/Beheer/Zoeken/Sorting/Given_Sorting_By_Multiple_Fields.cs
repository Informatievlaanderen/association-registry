namespace AssociationRegistry.Test.E2E.When_Searching.Beheer.Zoeken.Sorting;

using FluentAssertions;
using Framework.AlbaHost;
using Marten;
using System.Reflection;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(SearchCollection))]
public class Given_Sorting_By_Multiple_Fields
{
    private readonly SearchContext _testContext;
    private readonly ITestOutputHelper _testOutputHelper;

    public Given_Sorting_By_Multiple_Fields( SearchContext testContext, ITestOutputHelper testOutputHelper)

    {
        _testContext = testContext;
        _testOutputHelper = testOutputHelper;
    }

    [Theory(Skip = "")]
    [InlineData("verenigingstype.code", "naam")]
    [InlineData("verenigingstype.code", "korteNaam")]
    [InlineData("verenigingstype.code", "vCode")]
    public async Task Then_it_sorts_by_Verenigingstype_then_by_vCode_descending_V2(string ascendingField, string descendingField)
    {
        var result = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoeken(_testContext.ApiSetup.AdminHttpClient,
                                                                              $"*&sort={ascendingField},-{descendingField}",
                                                                              _testContext.ApiSetup.AdminApiHost.DocumentStore(), headers: new RequestParameters().V2().WithExpectedSequence(_testContext.MaxSequenceByScenario), testOutputHelper: _testOutputHelper);

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

    [Theory(Skip = "")]
    [InlineData("verenigingstype.code", "naam")]
    [InlineData("verenigingstype.code", "korteNaam")]
    [InlineData("verenigingstype.code", "vCode")]
    public async Task? Then_it_sorts_descending_then_ascending_V2(string descendingField, string ascendingField)
    {
        var result = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoeken(_testContext.ApiSetup.AdminHttpClient,
                                                                              $"*&sort=-{descendingField},{ascendingField}",
                                                                              _testContext.ApiSetup.AdminApiHost.DocumentStore(),
                                                                              headers: new RequestParameters().V2().WithExpectedSequence(_testContext.MaxSequenceByScenario), testOutputHelper: _testOutputHelper);

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
}
