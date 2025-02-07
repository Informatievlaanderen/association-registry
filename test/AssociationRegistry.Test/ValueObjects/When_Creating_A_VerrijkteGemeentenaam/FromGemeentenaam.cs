namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VerrijkteGemeentenaam;

using FluentAssertions;
using GemeentenaamDecorator;
using Vereniging;
using Xunit;

public class FromGemeentenaam
{
    [Theory]
    [InlineData("Bredene", "Bredene", null)]
    [InlineData("Buizingen (Halle)", "Halle", "Buizingen")]
    [InlineData(" Buizingen ( Halle ) ", "Halle", "Buizingen")]
    [InlineData(" Buizingen () ", " Buizingen () ", null)] // is invalid
    public void WithPostNaam_Then_An_Exception_Is_Thrown(string gemeentenaam, string expectedGemeentenaam, string expectedPostNaam)
    {
        var actual = VerrijkteGemeentenaam.FromGemeentenaam(new Gemeentenaam(gemeentenaam));
        actual.Gemeentenaam.Should().Be(expectedGemeentenaam);
        actual.Postnaam?.Value.Should().Be(expectedPostNaam);
    }
}
