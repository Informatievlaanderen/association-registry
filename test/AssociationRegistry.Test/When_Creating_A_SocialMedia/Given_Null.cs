﻿namespace AssociationRegistry.Test.When_Creating_A_SocialMedia;

using Contactgegevens.SocialMedias;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null
{
    [Fact]
    public void Then_it_returns_Null()
    {
        var website = SocialMedia.Create(null!);
        website.Should().BeNull();
    }
}
