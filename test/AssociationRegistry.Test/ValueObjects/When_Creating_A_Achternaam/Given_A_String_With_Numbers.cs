namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Achternaam;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

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
