namespace AssociationRegistry.Test.When_Creating_A_Website;

using FluentAssertions;
using Vereniging.Websites;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Valid_Value
{
    [Theory]
    [InlineData("http://www.my-domain.com")]
    [InlineData("https://www.my-domain.com")]
    [InlineData("https://www.my-other-domain.be")]
    [InlineData("https://www.sub.domain.be")]
    [InlineData("https://sub.domain.be")]
    [InlineData("https://domain.be")]
    public void Then_it_returns_a_Website(string? websiteString)
    {
        var website = Website.Create(websiteString);
        website.Waarde.Should().Be(websiteString);
    }
}
