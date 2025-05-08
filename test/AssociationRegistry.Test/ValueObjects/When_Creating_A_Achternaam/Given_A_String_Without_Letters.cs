namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Achternaam;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_String_Without_Letters
{
    [Theory]
    [InlineData("...---...")]
    [InlineData("@#//}{")]
    [InlineData("%%%")]
    public void Then_It_Throws_NumberInAchternaamException(string naamZonderLetters)
    {
        var create = () => Achternaam.Create(naamZonderLetters);
        create.Should().Throw<AchternaamZonderLetters>();
    }
}
