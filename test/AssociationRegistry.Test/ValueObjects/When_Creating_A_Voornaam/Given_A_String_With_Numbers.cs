namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Voornaam;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_String_With_Numbers
{
    [Theory]
    [InlineData("Mark the 1st")]
    [InlineData("n0el")]
    [InlineData("052125478")]
    public void Then_It_Throws_NumberInVoornaamException(string naamMetNummers)
    {
        var create = () => Voornaam.Create(naamMetNummers);
        create.Should().Throw<VoornaamBevatNummers>();
    }
}
