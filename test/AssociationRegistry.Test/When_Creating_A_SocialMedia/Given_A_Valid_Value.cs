namespace AssociationRegistry.Test.When_Creating_A_SocialMedia;

using ContactInfo.SocialMedias;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Valid_Value
{
    [Theory]
    [InlineData("http://www.my-domain.com")]
    [InlineData("https://www.my-other-domain.be")]
    public void Then_it_returns_a_SocialMedia(string? socialMediaString)
    {
        var socialMedia = SocialMedia.Create(socialMediaString);
        socialMedia.ToString().Should().Be(socialMediaString);
    }
}
