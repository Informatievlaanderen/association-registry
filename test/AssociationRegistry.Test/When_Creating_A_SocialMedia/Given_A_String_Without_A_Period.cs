﻿namespace AssociationRegistry.Test.When_Creating_A_SocialMedia;

using FluentAssertions;
using Vereniging.SocialMedias;
using Vereniging.SocialMedias.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
