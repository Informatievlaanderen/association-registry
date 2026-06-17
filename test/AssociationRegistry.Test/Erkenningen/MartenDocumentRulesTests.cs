namespace AssociationRegistry.Test.Erkenningen;

using AssociationRegistry.Admin.Schema.Erkenningen;
using FluentAssertions;
using Xunit;

public class MartenDocumentRulesTests
{
    [Fact]
    public void ErkenningDocument_Should_Not_Have_Nullable_Primitives()
    {
        var nullablePrimitiveProperties = typeof(ErkenningDocument)
            .GetProperties()
            .Where(property =>
            {
                var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);

                return underlyingType is not null && underlyingType != typeof(DateTimeOffset);
            })
            .Select(property => $"{property.Name}: {property.PropertyType.Name}");

        nullablePrimitiveProperties.Should().BeEmpty();
    }
}
