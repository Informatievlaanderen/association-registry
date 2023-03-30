namespace AssociationRegistry.Test.When_Creating_A_SocialMedia;

using ContactGegevens.SocialMedias;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Empty_String
{
    [Fact]
    public void Then_it_returns_Null()
    {
        var website = SocialMedia.Create(string.Empty);
        website.Should().BeNull();
    }
}
