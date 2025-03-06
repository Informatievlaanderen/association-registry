namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VerrijkteGemeentenaam;

using AssociationRegistry.Events;
using AssociationRegistry.Grar.Models.PostalInfo;
using GemeentenaamDecorator;
using System;
using Xunit;

public class WithEmptyOrNull
{
    [Theory]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("", null)]
    [InlineData(null, "Affligem")]
    [InlineData("Affligem", null)]
    public void WithPostNaam_Then_An_Exception_Is_Thrown(string postnaam, string gemeentenaam)
    {
        Assert.Throws<ArgumentException>(() => VerrijkteGemeentenaam.MetPostnaam(Postnaam.FromValue(postnaam), gemeentenaam));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void WithoutPostNaam_Then_An_Exception_Is_Thrown(string gemeentenaam)
    {
        Assert.Throws<ArgumentException>(() => VerrijkteGemeentenaam.ZonderPostnaam(gemeentenaam));
    }
}
