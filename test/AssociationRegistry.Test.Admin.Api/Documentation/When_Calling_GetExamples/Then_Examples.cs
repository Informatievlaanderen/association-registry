namespace AssociationRegistry.Test.Admin.Api.Documentation.When_Calling_GetExamples;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Then_Examples
{
    private readonly AdminApiFixture _fixture;

    private readonly string[] _allowedNullProperties =
    {
        "Vereniging.Roepnaam",
        "Verenigingen.Roepnaam",
        "Vereniging.Locaties.Adres.Busnummer",
        "Vereniging.Locaties.AdresId",
        "Vereniging.Einddatum",
        "Verenigingen.Einddatum",
    };

    public Then_Examples(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Do_not_have_null_values()
    {
        var types = typeof(Program).Assembly
                                   .GetTypes()
                                   .Where(x => !x.IsAbstract &&
                                               x.GetInterfaces()
                                                .Any(y => y.IsGenericType &&
                                                          y.GetGenericTypeDefinition() == typeof(IExamplesProvider<>)))
                                   .ToList();

        using var context = new AssertionScope();

        foreach (var type in types)
        {
            var provider = _fixture.ServiceProvider.GetRequiredService(type);

            var getExamplesMethod = type.GetMethod("GetExamples");

            var examples = getExamplesMethod.Invoke(provider, parameters: null);

            CheckAllPropertiesNotNull(examples, type);
        }
    }

    private void CheckAllPropertiesNotNull(object examples, Type type)
    {
        if (examples is IEnumerable exampleEnumerable)
            foreach (var example in exampleEnumerable)
            {
                CheckAllPropertiesNotNull(example, type);
            }
        else
            examples.ShouldHaveAllPropertiesNotNull(
                type.Name,
                string.Empty,
                _allowedNullProperties);
    }
}

public static class NotNullExtensions
{
    public static void ShouldHaveAllPropertiesNotNull(this object obj, string baseType, string? parentName, params string[] allowedNulls)
    {
        foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!property.CanRead) continue;

            if (property.GetIndexParameters().Length > 0) continue;

            var fullPath =
                string.IsNullOrEmpty(parentName) ? property.Name : parentName + "." + property.Name;

            if (allowedNulls is not null && allowedNulls.Contains(fullPath)) continue;

            var propertyValue = property.GetValue(obj, index: null);
            propertyValue.Should().NotBeNull($"because property '{fullPath}' should not be null on type '{baseType}'.");

            var propertyType = property.PropertyType;

            // Check if the property is a value type or string (since string is immutable and doesn't have properties to check further)
            if (propertyType.IsValueType || propertyType == typeof(string))
                continue;

            // Check if the property type implements IEnumerable (and is not a string)
            if (typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                foreach (var item in (IEnumerable)propertyValue)
                {
                    item.ShouldHaveAllPropertiesNotNull(baseType, fullPath, allowedNulls);
                }

                continue;
            }

            // Recurse into complex properties
            propertyValue.ShouldHaveAllPropertiesNotNull(baseType, fullPath, allowedNulls);
        }
    }
}
