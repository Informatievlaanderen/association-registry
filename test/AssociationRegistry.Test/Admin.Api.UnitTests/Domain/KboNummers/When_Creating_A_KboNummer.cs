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
        [InlineData("0000000000", "0000000000")]
        [InlineData("1111111111", "1111111111")]
        [InlineData("1234.123.123", "1234123123")]
        [InlineData("1234 123 123", "1234123123")]
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
        public void Then_it_throws_an_InvalidKboNummerException(string kboNummerString)
        {
            var factory = () => KboNummer.Create(kboNummerString);
            factory.Should().Throw<InvalidKboNummer>();
        }
    }

    public class Given_A_String_With_Non_Numeric_Characters
    {
        [Theory]
        [InlineData("AAAABBBCCC")]
        [InlineData("AAAA123CCC")]
        [InlineData("-209850349")]
        [InlineData("%$&*)(*&⁽)")]
        public void Then_it_throws_an_InvalidKboNummerException(string kboNummerString)
        {
            var factory = () => KboNummer.Create(kboNummerString);
            factory.Should().Throw<InvalidKboNummer>();
        }
    }
}
