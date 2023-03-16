﻿namespace AssociationRegistry.Test.When_Creating_An_Insz;

using FluentAssertions;
using INSZ;
using INSZ.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_With_Wrong_Length
{
    [Theory]
    [InlineData("01234567890123")]
    [InlineData("012345678")]
    [InlineData("01.23.45-678.9")]
    [InlineData("01.23.45--678.9")]
    [InlineData("01.23.45-678-9012")]
    [InlineData("01.23..45-678-9012")]
    public void Then_it_throws_an_InvalidInszLengthException(string insz)
    {
        var factory = () => Insz.Create(insz);
        factory.Should().Throw<InvalidInszLength>();
    }
}
