namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Voornaam;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_String_Without_Letters
{
    [Theory]
    [InlineData("...---...")]
    [InlineData("@#//}{")]
    [InlineData("%%%")]
    public void Then_It_Throws_NumberInVoornaamException(string naamZonderLetters)
    {
        var create = () => Voornaam.Create(naamZonderLetters);
        create.Should().Throw<VoornaamZonderLetters>();
    }
}
