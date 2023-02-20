namespace AssociationRegistry.Test.When_Creating_A_SocialMedia;

using ContactInfo.SocialMedias;
using FluentAssertions;
using Xunit;

public class Given_An_Empty_String
{
    [Fact]
    public void Then_it_returns_Null()
    {
        var socialMedia = SocialMedia.Create(string.Empty);
        socialMedia.Should().BeNull();
    }
}
