namespace AssociationRegistry.Test.When_Creating_A_LocatieLijst;

using FluentAssertions;
using Locaties;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Empty_Array
{
    [Fact]
    public void Then_It_Returns_Empty()
    {
        var locaties = LocatieLijst.CreateInstance(Array.Empty<Locatie>());
        locaties.Should().BeEmpty();
    }
}
