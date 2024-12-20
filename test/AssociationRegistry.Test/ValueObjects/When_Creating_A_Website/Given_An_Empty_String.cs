namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Website;

using AssociationRegistry.Vereniging.Websites;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Empty_String
{
    [Fact]
    public void Then_it_returns_Null()
    {
        var website = Website.Create(string.Empty);
        website.Should().Be(Website.Leeg);
    }
}
