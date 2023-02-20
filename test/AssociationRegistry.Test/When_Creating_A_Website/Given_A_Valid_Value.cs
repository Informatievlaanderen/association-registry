namespace AssociationRegistry.Test.When_Creating_A_Website;

using ContactInfo.Websites;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Valid_Value
{
    [Theory]
    [InlineData("http://www.my-domain.com")]
    [InlineData("https://www.my-other-domain.be")]
    public void Then_it_returns_a_Website(string? websiteString)
    {
        var website = Website.Create(websiteString);
        website.ToString().Should().Be(websiteString);
    }
}
