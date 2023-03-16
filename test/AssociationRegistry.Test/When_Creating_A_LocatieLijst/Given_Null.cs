namespace AssociationRegistry.Test.When_Creating_A_LocatieLijst;

using Locaties;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null
{
    [Fact]
    public void Then_It_Returns_Empty()
    {
        var locaties = LocatieLijst.CreateInstance(null);
        locaties.Should().BeEmpty();
    }
}
