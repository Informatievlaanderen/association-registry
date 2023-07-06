﻿namespace AssociationRegistry.Test.When_Creating_A_Doelgroep;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Value_Outside_Allowed_Range
{
    [Theory]
    [InlineData(-5, -1)]
    [InlineData(-1, 10)]
    [InlineData(-2, 151)]
    [InlineData(5, 200)]
    [InlineData(160, 200)]
    public void Then_it_Throws_An_DoelgroepOutOfRange(int min, int max)
    {
        var create = () => Doelgroep.Create(min, max);
        create.Should().Throw<DoelgroepOutOfRange>();
    }
}
