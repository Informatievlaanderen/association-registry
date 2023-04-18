namespace AssociationRegistry.Test.When_Creating_A_Website;

using FluentAssertions;
using Vereniging.Websites;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null
{
    [Fact]
    public void Then_it_returns_Null()
    {
        var website = Website.Create(null!);
        website.Should().Be(Website.Leeg);
    }
}
