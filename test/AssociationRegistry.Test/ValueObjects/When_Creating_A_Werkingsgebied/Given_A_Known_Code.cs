﻿namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Werkingsgebied;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Known_Code
{
    [Theory]
    [InlineData("NVT")]
    [InlineData("BE25")]
    [InlineData("BE25535002")]
    public void Then_identical_codes_with_correct_casing_return_a_werkingsgebied(string code)
    {
        var ctor = () => Werkingsgebied.Create(code);

        ctor.Should().NotThrow<WerkingsgebiedCodeIsNietGekend>();

        Werkingsgebied.Create(code).Should().NotBeNull();
    }

    [Theory]
    [InlineData("nvt")]
    [InlineData("be25")]
    public void Then_identical_codes_with_incorrect_casing_return_a_werkingsgebied(string code)
    {
        var ctor = () => Werkingsgebied.Create(code);

        ctor.Should().NotThrow<WerkingsgebiedCodeIsNietGekend>();

        Werkingsgebied.Create(code).Should().NotBeNull();
    }
}
