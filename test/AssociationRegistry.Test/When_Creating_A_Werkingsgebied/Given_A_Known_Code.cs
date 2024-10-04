﻿namespace AssociationRegistry.Test.When_Creating_A_Werkingsgebied;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Known_Code
{
    [Theory]
    [InlineData("BE25")]
    [InlineData("BE255")]
    [InlineData("BE25535002")]
    public void Then_identical_codes_with_correct_casing_return_a_werkingsgebied(string code)
    {
        var ctor = () => Werkingsgebied.Create(code);

        ctor.Should().NotBeNull();
        ctor.Should().NotThrow<WerkingsgebiedCodeIsNietGekend>();
    }

    [Theory]
    [InlineData("be")]
    [InlineData("be2")]
    [InlineData("be25")]
    public void Then_identical_codes_with_incorrect_casing_return_a_werkingsgebied(string code)
    {
        var ctor = () => Werkingsgebied.Create(code);

        ctor.Should().NotThrow();
        ctor.Should().NotThrow<WerkingsgebiedCodeIsNietGekend>();
    }
}