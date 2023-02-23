namespace AssociationRegistry.Test.When_Creating_A_Url;

using ContactInfo.Urls;
using FluentAssertions;
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
        var website = Url.Create(websiteString);
        website.ToString().Should().Be(websiteString);
    }
}
