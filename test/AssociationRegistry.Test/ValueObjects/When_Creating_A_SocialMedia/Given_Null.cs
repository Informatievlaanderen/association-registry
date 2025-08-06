namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_SocialMedia;

using DecentraalBeheer.Vereniging.SocialMedias;
using FluentAssertions;
using Xunit;

public class Given_Null
{
    [Fact]
    public void Then_it_returns_Leeg()
    {
        var socialMedia = SocialMedia.Create(null!);
        socialMedia.Should().Be(SocialMedia.Leeg);
    }
}
