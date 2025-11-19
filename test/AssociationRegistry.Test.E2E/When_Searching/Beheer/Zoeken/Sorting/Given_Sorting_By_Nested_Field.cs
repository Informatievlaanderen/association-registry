namespace AssociationRegistry.Test.E2E.When_Searching.Beheer.Zoeken.Sorting;

using FluentAssertions;
using Framework.AlbaHost;
using Marten;
using System.Reflection;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(SearchCollection))]
public class Given_Sorting_By_Nested_Fields
{
    private readonly SearchContext _testContext;
    private readonly ITestOutputHelper _helper;

    public Given_Sorting_By_Nested_Fields(SearchContext testContext, ITestOutputHelper helper)
    {
        _testContext = testContext;
        _helper = helper;
    }

    [Fact(Skip = "")]
    public async ValueTask Then_it_sorts_ascending()
    {
        var field = "doelgroep.minimumleeftijd";

        var result = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoeken(_testContext.ApiSetup.AdminHttpClient,
                                                                              "*", _testContext.ApiSetup.AdminApiHost.DocumentStore(), "doelgroep.minimumleeftijd",

                                                                              headers: new RequestParameters().V2()
                                                                                 .WithExpectedSequence(_testContext.MaxSequenceByScenario),
                                                                              testOutputHelper: _helper);

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
        var result = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoeken(_testContext.ApiSetup.AdminHttpClient,
                                                                              "*",
                                                                              _testContext.ApiSetup.AdminApiHost.DocumentStore(), sort: "-doelgroep.minimumleeftijd",

                                                                              new RequestParameters()
                                                                                  .V2()
                                                                                 .WithExpectedSequence(_testContext.MaxSequenceByScenario),
                                                                              testOutputHelper: _helper);

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
