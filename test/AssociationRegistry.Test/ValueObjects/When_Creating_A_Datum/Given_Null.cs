namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Datum;

using AssociationRegistry.Vereniging;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null
{
    [Fact]
    public void Then_it_returns_null()
    {
        var datum = Datum.CreateOptional(null!);
        datum.Should().BeNull();
    }
}
