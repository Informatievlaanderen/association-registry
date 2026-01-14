namespace AssociationRegistry.Test.Bankrekeningnummers.When_Creating_Iban_Nummer;

using DecentraalBeheer.Vereniging.Bankrekeningen;
using DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Invalid_Iban
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    [InlineData("BE68")]
    [InlineData("BE6853900754703")]
    [InlineData("BE685390075470340")]
    [InlineData("BE68-5390-0754-7034")]
    [InlineData("BE68_5390_0754_7034")]
    [InlineData("BE68A39007547034")]
    [InlineData("NL68539007547034")]
    [InlineData("BE68539007547035")]
    [InlineData("XX68539007547034")]
    [InlineData(".BE68539007547034")]
    [InlineData(" BE68539007547034")]
    public void Then_IbanNummer_Is_Created_And_Senatized(string iban)
    {
       var exceptions = Assert.Throws<IbanFormaatIsOngeldig>(() => IbanNummer.Create(iban));
       exceptions.Message.Should().Be(ExceptionMessages.IbanFormaatIsOngeldig);
    }
}
