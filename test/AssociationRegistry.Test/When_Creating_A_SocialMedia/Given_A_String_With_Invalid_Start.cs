﻿namespace AssociationRegistry.Test.When_Creating_A_SocialMedia;

using FluentAssertions;
using Vereniging.SocialMedias;
using Vereniging.SocialMedias.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_With_Invalid_Start
{
    [Theory]
    [InlineData("bla.bla.bla")]
    [InlineData("http:/oeps.com")]
    [InlineData("www.hello.me")]
    public void Then_it_throws_InvalidWebsiteStartException(string? invalidWebsiteString)
    {
        var ctor = () => SocialMedia.Create(invalidWebsiteString);

        ctor.Should().Throw<SocialMediaMoetStartenMetHttp>();
    }
}
