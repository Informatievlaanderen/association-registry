namespace AssociationRegistry.Test.When_Creating_A_SocialMedia;

using FluentAssertions;
using Vereniging.SocialMedias;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null
{
    [Fact]
    public void Then_it_returns_Leeg()
    {
        var socialMedia = SocialMedia.Create(null!);
        socialMedia.Should().Be(SocialMedia.Leeg);
    }
}
