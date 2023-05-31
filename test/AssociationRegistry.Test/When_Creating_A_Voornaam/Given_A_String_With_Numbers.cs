namespace AssociationRegistry.Test.When_Creating_A_Voornaam;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
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
