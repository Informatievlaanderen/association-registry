namespace AssociationRegistry.Test.ValueObjects.When_Creating_An_Iban_Nummer;

using DecentraalBeheer.Vereniging.Bankrekeningen;
using DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_An_Invalid_Iban
{
    [Theory]
    [InlineData("AABBCCDDDEE")]
    [InlineData("12AB34CDE56")]
    [InlineData("%$&*)(*&⁽)@")]
    [InlineData("DE68539007547034")]
    [InlineData("be68539007547034")]
    [InlineData("BE6853900754703")]
    [InlineData("BE685390075470345")]
    [InlineData("BE000000000")]
    [InlineData("BE00000000000000")]
    [InlineData("BE12ABC456789012")]
    [InlineData("BE12-3456-7890-12")]
    [InlineData("XBE68539007547034")]
    public void With_Non_Numeric_Characters_Then_it_throws_an_IbanFormaatIsOngeldig(string iban)
    {
        var factory = () => IbanNummer.Create(iban);
        factory.Should().Throw<IbanFormaatIsOngeldig>();
    }
}
