namespace AssociationRegistry.Test.E2E.When_Searching.Publiek.Zoeken.Sorting;

using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using FluentAssertions;
using System.Reflection;
using Xunit;

[Collection(nameof(SearchCollection))]
public class Given_Sorting_By_Multiple_Fields
{
    private readonly SearchContext _testContext;

    public Given_Sorting_By_Multiple_Fields( SearchContext testContext)

    {
        _testContext = testContext;
    }

    [Theory]
    [InlineData("verenigingstype.code", "naam")]
    [InlineData("verenigingstype.code", "korteNaam")]
    [InlineData("verenigingstype.code", "vCode")]
    public async Task Then_it_sorts_by_Verenigingstype_then_by_vCode_descending_V2(string ascendingField, string descendingField)
    {
        var result = await _testContext.ApiSetup.PublicApiHost.GetPubliekZoekenWithHeader(_testContext.ApiSetup.SuperAdminHttpClient,
                                                                                $"{ExcludeFV()}&sort={ascendingField},-{descendingField}", _testContext.MaxSequenceByScenario);

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
                                                                                          $"{ExcludeFV()}&sort=-{descendingField},{ascendingField}", _testContext.MaxSequenceByScenario);

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

    // Do this to prevent legacy data messing with the sort
    private static string ExcludeFV()
        => "NOT verenigingstype.code:FV";

    private static object GetPropertyValue(object obj, string propertyName)
    {
        var propInfo = obj.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        return propInfo?.GetValue(obj, null);
    }
}
