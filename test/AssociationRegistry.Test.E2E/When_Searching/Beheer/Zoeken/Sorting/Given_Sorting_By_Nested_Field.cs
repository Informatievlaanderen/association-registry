﻿namespace AssociationRegistry.Test.E2E.When_Searching.Beheer.Zoeken.Sorting;

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
public class Given_Sorting_By_Nested_Fields : End2EndTest<SearchContext, NullRequest, SearchVerenigingenResponse>
{
    private readonly SearchContext _testContext;

    public Given_Sorting_By_Nested_Fields(SearchContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Theory]
    //[InlineData("verenigingstype.code")]
    [InlineData("doelgroep.minimumleeftijd")]
    public async Task? Then_it_sorts_ascending(string field)
    {
        var result = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoekenV2(
            _testContext.ApiSetup.SuperAdminHttpClient, $"*&sort={field}");

        var values = result.Verenigingen
                           .Select(x => GetNestedPropertyValue(x, field))
                           .ToList();

        values.Should().NotBeEmpty();
        values.Should().BeInAscendingOrder();
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

    [Theory]
    [InlineData("verenigingstype.code", "naam")]
    [InlineData("verenigingstype.code", "korteNaam")]
    [InlineData("verenigingstype.code", "vCode")]
    public async Task Then_it_sorts_by_Verenigingstype_then_by_vCode_descending_V2(string ascendingField, string descendingField)
    {
        var result = await _testContext.ApiSetup.AdminApiHost.GetBeheerZoekenV2(_testContext.ApiSetup.SuperAdminHttpClient,
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

    private static object GetPropertyValue(object obj, string propertyName)
    {
        var propInfo = obj.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        return propInfo?.GetValue(obj, null);
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerZoeken("*&sort=verenigingstype.code");
}
