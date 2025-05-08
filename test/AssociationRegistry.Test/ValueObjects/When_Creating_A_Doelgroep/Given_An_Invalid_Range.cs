namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Doelgroep;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_An_Invalid_Range
{
    [Theory]
    [InlineData(50, 2)]
    [InlineData(150, 0)]
    [InlineData(11, 10)]
    public void Then_it_Throws_An_InvalidDoelgroepRange(int min, int max)
    {
        var create = () => Doelgroep.Create(min, max);
        create.Should().Throw<MinimumLeeftijdMoetKleinerOfGelijkZijnAanMaximumLeeftijd>();
    }
}
