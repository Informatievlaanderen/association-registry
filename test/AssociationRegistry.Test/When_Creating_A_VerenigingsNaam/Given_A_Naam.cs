﻿namespace AssociationRegistry.Test.When_Creating_A_VerenigingsNaam;

using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Naam
{
    [Theory]
    [InlineData("Vereniging A")]
    [InlineData("Jabedabedoe")]
    [InlineData("Vereniging zonder naam")]
    [InlineData("123456789")]
    public void Then_It_Returns_A_New_VerenigingsNaam(string naam)
    {
        var verenigingsNaam = VerenigingsNaam.Create(naam);
        verenigingsNaam.ToString().Should().Be(naam);
    }
}
