﻿namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Werkingsgebied;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Unknown_Code
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Unknown")]
    [InlineData("RANDOM")]
    public void Then_it_throws_WerkingsgebiedCodeIsNietGekend(string code)
    {
        var ctor = () => Werkingsgebied.Create(code);

        ctor.Should().Throw<WerkingsgebiedCodeIsNietGekend>();
    }
}
