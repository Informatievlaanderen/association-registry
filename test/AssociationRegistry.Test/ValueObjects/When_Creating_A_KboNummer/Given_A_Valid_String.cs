namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_KboNummer;

using AssociationRegistry.Vereniging;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Valid_String
{
    [Theory]
    [InlineData("0000000097", "0000000097")]
    [InlineData("1111111145", "1111111145")]
    [InlineData("1234.123.179", "1234123179")]
    [InlineData("1234 123 179", "1234123179")]
    [InlineData("0000 000.097", "0000000097")]
    [InlineData("1111.111 145", "1111111145")]
    [InlineData("123.1564.260", "1231564260")]
    [InlineData("12.34.56.78.94", "1234567894")]
    [InlineData(".0123456749", "0123456749")]
    [InlineData("0123456749.", "0123456749")]
    [InlineData("123 1564 260", "1231564260")]
    [InlineData("12 34 56 78 94", "1234567894")]
    [InlineData(" 0123456749", "0123456749")]
    [InlineData("0123456749 ", "0123456749")]
    [InlineData("0746889508", "0746889508")]
    public void Then_it_returns_a_kboNummer(string kboNummerString, string expectedKboNummer)
    {
        string kboNummer = KboNummer.Create(kboNummerString);

        kboNummer.Should().Be(expectedKboNummer);
    }
}
