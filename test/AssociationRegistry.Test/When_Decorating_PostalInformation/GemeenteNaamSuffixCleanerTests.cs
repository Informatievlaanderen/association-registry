﻿namespace AssociationRegistry.Test.When_Decorating_PostalInformation;

using FluentAssertions;
using Vereniging;
using Xunit;

public class GemeenteNaamSuffixCleanerTests
{
    [Theory]
    [InlineData("Hekelgem (Affligem)")]
    [InlineData("Hekelgem (afg)")]
    [InlineData("Hekelgem")]
    [InlineData(" Hekelgem")]
    [InlineData("Hekelgem ")]
    [InlineData(" Hekelgem ")]
    public void RemoveBracketsAndContent(string input)
    {
        var sut = GemeenteNaamSuffixCleanerRegex.Instance;

        var actual = sut.Clean(Gemeentenaam.FromValue(input));
        actual.Should().Be("Hekelgem");
    }
}
