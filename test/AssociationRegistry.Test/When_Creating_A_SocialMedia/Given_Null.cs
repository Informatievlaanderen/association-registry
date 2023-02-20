namespace AssociationRegistry.Test.When_Creating_A_SocialMedia;

using ContactInfo.SocialMedias;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null
{
    [Fact]
    public void Then_it_returns_Null()
    {
        var socialMedia = SocialMedia.Create(null!);
        socialMedia.Should().BeNull();
    }
}
