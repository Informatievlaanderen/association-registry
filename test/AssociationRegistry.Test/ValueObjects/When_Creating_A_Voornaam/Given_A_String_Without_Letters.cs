namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Voornaam;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
