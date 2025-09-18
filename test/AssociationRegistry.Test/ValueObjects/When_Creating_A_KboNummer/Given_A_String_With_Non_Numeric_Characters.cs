namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_KboNummer;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_String_With_Non_Numeric_Characters
{
    [Theory]
    [InlineData("AAAABBBCCC")]
    [InlineData("AAAA123CCC")]
    [InlineData("-209850349")]
    [InlineData("%$&*)(*&⁽)")]
    public void Then_it_throws_an_InvalidKboNummerCharsException(string kboNummerString)
    {
        var factory = () => KboNummer.Create(kboNummerString);
        factory.Should().Throw<KboNummerBevatOngeldigeTekens>();
    }
}
