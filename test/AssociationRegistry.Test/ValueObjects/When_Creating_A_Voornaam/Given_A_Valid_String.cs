namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Voornaam;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_A_Valid_String
{
    [Theory]
    [InlineData("Jef")]
    [InlineData("Gert-Jan")]
    [InlineData("X Æ A-Xii")]
    public void Then_It_Returns_A_Voornaam(string naam)
    {
        var voornaam = Voornaam.Create(naam);
        voornaam.Should().NotBeNull();
    }
}
