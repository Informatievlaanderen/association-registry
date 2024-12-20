namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_KboNummer;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_With_Wrong_Length
{
    [Theory]
    [InlineData("000000000051")]
    [InlineData("1234.123.5123")]
    [InlineData("1234 1215 123")]
    [InlineData("11111111")]
    [InlineData("1234.123.23")]
    [InlineData("1234 12 123")]
    public void Then_it_throws_an_InvalidKboNummerLengthException(string kboNummerString)
    {
        var factory = () => KboNummer.Create(kboNummerString);
        factory.Should().Throw<KboNummerLengteIsOngeldig>();
    }
}
