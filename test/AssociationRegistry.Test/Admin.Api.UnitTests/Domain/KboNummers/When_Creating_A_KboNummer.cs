namespace AssociationRegistry.Test.Admin.Api.UnitTests.Domain.KboNummers;

using AssociationRegistry.Admin.Api.Verenigingen.KboNummers;
using AssociationRegistry.Admin.Api.Verenigingen.KboNummers.Exceptions;
using FluentAssertions;
using Xunit;

public class When_Creating_A_KboNummer
{
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
            var kboNummer = KboNummer.Create(kboNummerString)!;

            kboNummer.Value.Should().Be(expectedKboNummer);
        }
    }

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
            factory.Should().Throw<InvalidKboNummerLength>();
        }
    }

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
            factory.Should().Throw<InvalidKboNummerChars>();
        }
    }

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

    public class Given_Null_KboNummer
    {
        [Fact]
        public void Then_it_returns_null()
        {
            var kboNummer = KboNummer.Create(null);

            kboNummer.Should().BeNull();
        }
    }
}
