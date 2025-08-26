namespace AssociationRegistry.Test.E2E.When_Searching.Beheer.Zoeken.Sorting;

using FluentAssertions;
using Framework.AlbaHost;
using Marten;
using System.Reflection;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(SearchCollection))]
public class Given_Sorting_By_Single_Field
{
    private readonly SearchContext _testContext;
    private readonly ITestOutputHelper _testOutputHelper;

    public Given_Sorting_By_Single_Field( SearchContext testContext, ITestOutputHelper testOutputHelper)

    {
        _testContext = testContext;
        _testOutputHelper = testOutputHelper;
    }



    [Theory]
    [InlineData("naam")]
    [InlineData("korteNaam")]
    [InlineData("vCode")]
    public async Task Then_it_sorts_descending_V2(string field)
    {
        var result = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoeken(_testContext.ApiSetup.AdminHttpClient,
                                                                              $"*&sort=-{field}",
                                                                              _testContext.ApiSetup.AdminApiHost.DocumentStore(),headers: new RequestParameters().V2().WithExpectedSequence(_testContext.MaxSequenceByScenario), testOutputHelper: _testOutputHelper);

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
        var result = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoeken(_testContext.ApiSetup.AdminHttpClient,
                                                                              $"*&sort={field}",
                                                                              _testContext.ApiSetup.AdminApiHost.DocumentStore(),
                                                                              headers: new RequestParameters().V2().WithExpectedSequence(_testContext.MaxSequenceByScenario), testOutputHelper: _testOutputHelper);

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
