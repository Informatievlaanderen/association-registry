namespace AssociationRegistry.Test.When_Creating_A_SocialMedia;

using ContactInfo.SocialMedias;
using ContactInfo.SocialMedias.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_With_Invalid_Start
{
    [Theory]
    [InlineData("bla.bla.bla")]
    [InlineData("http:/oeps.com")]
    [InlineData("www.hello.me")]
    public void Then_it_throws_InvalidSocialMediaStartException(string? invalidSocialMediaString)
    {
        var ctor = () => SocialMedia.Create(invalidSocialMediaString);

        ctor.Should().Throw<InvalidSocialMediaStart>();
    }
}
