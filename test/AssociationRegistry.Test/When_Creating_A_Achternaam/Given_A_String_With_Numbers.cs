﻿namespace AssociationRegistry.Test.When_Creating_A_Achternaam;

using Vereniging;
using Vereniging.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_With_Numbers
{
    [Theory]
    [InlineData("Mark the 1st")]
    [InlineData("n0el")]
    [InlineData("052125478")]
    public void Then_It_Throws_NumberInAchternaamException(string naamMetNummers)
    {
        var create = () => Achternaam.Create(naamMetNummers);
        create.Should().Throw<AchternaamBevatNummers>();
    }
}
