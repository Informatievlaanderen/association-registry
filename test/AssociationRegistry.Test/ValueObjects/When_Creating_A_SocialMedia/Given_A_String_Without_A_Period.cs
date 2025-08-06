namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_SocialMedia;

using DecentraalBeheer.Vereniging.SocialMedias;
using DecentraalBeheer.Vereniging.SocialMedias.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_String_Without_A_Period
{
    [Theory]
    [InlineData("http://awebsitewithoutperiods")]
    [InlineData("https://gibberish")]
    public void Then_it_throws_WebsiteMissingPeriodException(string? invalidWebsiteString)
    {
        var ctor = () => SocialMedia.Create(invalidWebsiteString);

        ctor.Should().Throw<SocialMoetMinstensEenPuntBevatten>();
    }
}
