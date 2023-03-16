namespace AssociationRegistry.Test.When_Creating_A_KboNummer;

using FluentAssertions;
using KboNummers;
using KboNummers.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Incorrect_Modulo97
{
    [Theory]
    [InlineData("0000000096")]
    [InlineData("1131111145")]
    [InlineData("1234123175")]
    public void Then_it_throws_an_InvalidKboNummerMod97Exception(string kboNummerString)
    {
        var factory = () => KboNummer.Create(kboNummerString);
        factory.Should().Throw<InvalidKboNummerMod97>();
    }
}
