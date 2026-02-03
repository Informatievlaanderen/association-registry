namespace AssociationRegistry.Test.Bankrekeningnummers.When_Creating_Iban_Nummer;

using DecentraalBeheer.Vereniging.Bankrekeningen;
using FluentAssertions;
using Xunit;

public class Given_Valid_Iban
{
    [Theory]
    [InlineData("BE68539007547034")]
    [InlineData("BE68 5390 0754 7034")]
    [InlineData("BE68.5390.0754.7034")]
    public void Then_IbanNummer_Is_Created_And_Senatized(string iban)
    {
        var ibanNummer = IbanNummer.Create(iban);

        ibanNummer.Value.Should().Be("BE68 5390 0754 7034");
    }
}
