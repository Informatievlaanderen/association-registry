namespace AssociationRegistry.Test.E2E.When_Searching.Publiek.Zoeken.Sorting;

using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using FluentAssertions;
using System.Reflection;
using Xunit;

[Collection(nameof(SearchCollection))]
public class Given_Sorting_By_Single_Field
{
    private readonly SearchContext _testContext;

    public Given_Sorting_By_Single_Field( SearchContext testContext)

    {
        _testContext = testContext;
    }

    [Theory]
    [InlineData("naam")]
    [InlineData("korteNaam")]
    [InlineData("vCode")]
    public async Task Then_it_sorts_descending_V2(string field)
    {
        var result = await _testContext.ApiSetup.PublicApiHost.GetPubliekZoekenWithHeader(_testContext.ApiSetup.SuperAdminHttpClient,
                                                                                $"*&sort=-{field}", _testContext.MaxSequenceByScenario);

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
                                                                                $"*&sort={field}", _testContext.MaxSequenceByScenario);

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
}
