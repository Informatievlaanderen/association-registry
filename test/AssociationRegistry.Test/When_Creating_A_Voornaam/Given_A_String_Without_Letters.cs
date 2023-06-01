namespace AssociationRegistry.Test.When_Creating_A_Voornaam;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
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
