namespace AssociationRegistry.Test.When_Creating_A_Achternaam;

using Vereniging;
using Vereniging.Exceptions;
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
    public void Then_It_Throws_NumberInAchternaamException(string naamZonderLetters)
    {
        var create = () => Achternaam.Create(naamZonderLetters);
        create.Should().Throw<AchternaamZonderLetters>();
    }
}
