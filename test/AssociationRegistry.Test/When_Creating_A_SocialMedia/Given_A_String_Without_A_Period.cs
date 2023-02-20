namespace AssociationRegistry.Test.When_Creating_A_SocialMedia;

using ContactInfo.SocialMedias;
using ContactInfo.SocialMedias.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_Without_A_Period
{
    [Theory]
    [InlineData("http://awebsitewithoutperiods")]
    [InlineData("https://gibberish")]
    public void Then_it_throws_WebsiteMissingPeriodException(string? invalidSocialMediaString)
    {
        var ctor = () => SocialMedia.Create(invalidSocialMediaString);

        ctor.Should().Throw<SocialMediaMissingPeriod>();
    }
}
