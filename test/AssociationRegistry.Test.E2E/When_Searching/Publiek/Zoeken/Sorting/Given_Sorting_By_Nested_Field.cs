namespace AssociationRegistry.Test.E2E.When_Searching.Publiek.Zoeken.Sorting;

using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using FluentAssertions;
using System.Reflection;
using Xunit;

[Collection(nameof(SearchCollection))]
public class Given_Sorting_By_Nested_Fields
{
    private readonly SearchContext _testContext;

    public Given_Sorting_By_Nested_Fields( SearchContext testContext)

    {
        _testContext = testContext;
    }



    [Fact(Skip = "")]
    public async ValueTask Then_it_sorts_ascending()
    {
        var field = "doelgroep.minimumleeftijd";

        var result = await _testContext.ApiSetup.PublicApiHost.GetPubliekZoekenWithHeader(
            _testContext.ApiSetup.SuperAdminHttpClient, $"*&sort=doelgroep.minimumleeftijd", _testContext.MaxSequenceByScenario);

        var values = result.Verenigingen
                           .Select(x => GetNestedPropertyValue(x, field))
                           .ToList();

        values.Should().NotBeEmpty();
        values.Should().BeInAscendingOrder();
    }

    [Fact(Skip = "")]
    public async ValueTask Then_it_sorts_descending()
    {
        var field = "doelgroep.minimumleeftijd";
        var result = await _testContext.ApiSetup.PublicApiHost.GetPubliekZoekenWithHeader(
            _testContext.ApiSetup.SuperAdminHttpClient, $"*&sort=-doelgroep.minimumleeftijd", _testContext.MaxSequenceByScenario);

        var values = result.Verenigingen
                           .Select(x => GetNestedPropertyValue(x, field))
                           .ToList();

        values.Should().NotBeEmpty();
        values.Should().BeInDescendingOrder();
    }

    private static object? GetNestedPropertyValue(object obj, string propertyPath)
    {
        var props = propertyPath.Split('.');
        object? currentObject = obj;

        foreach (var prop in props)
        {
            if (currentObject == null) return null;

            var type = currentObject.GetType();
            var property = type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                throw new ArgumentException($"Property '{prop}' not found on type '{type.Name}'");

            currentObject = property.GetValue(currentObject, null);
        }

        return currentObject;
    }
}
