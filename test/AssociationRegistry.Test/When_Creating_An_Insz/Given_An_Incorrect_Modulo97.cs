namespace AssociationRegistry.Test.When_Creating_An_Insz;

using FluentAssertions;
using INSZ;
using INSZ.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Incorrect_Modulo97
{
    [Theory]
    [InlineData("01234567890")]
    [InlineData("00000000000")]
    [InlineData("01.23.45-678.90")]
    [InlineData("01.23.45--678.90")]
    [InlineData("01.23..45-678-90")]
    public void Then_it_throws_an_InvalidInszMod97Exception(string insz)
    {
        var factory = () => Insz.Create(insz);
        factory.Should().Throw<InvalidInszMod97>();
    }
}
