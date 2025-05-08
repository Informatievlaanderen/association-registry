namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Website;

using AssociationRegistry.Vereniging.Websites;
using FluentAssertions;
using Xunit;

public class Given_A_Valid_Value
{
    [Theory]
    [InlineData("http://www.my-domain.com")]
    [InlineData("https://www.my-domain.com")]
    [InlineData("https://www.my-other-domain.be")]
    [InlineData("https://www.sub.domain.be")]
    [InlineData("https://sub.domain.be")]
    [InlineData("https://domain.be")]
    [InlineData("HTTPS://DOMAIN.BE")]
    public void Then_it_returns_a_Website(string? websiteString)
    {
        var website = Website.Create(websiteString);
        website.Waarde.Should().Be(websiteString);
    }
}
