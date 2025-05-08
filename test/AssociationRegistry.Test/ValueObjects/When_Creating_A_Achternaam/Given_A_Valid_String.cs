namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Achternaam;

using AssociationRegistry.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_A_Valid_String
{
    [Theory]
    [InlineData("Jef")]
    [InlineData("Gert-Jan")]
    [InlineData("X Æ A-Xii")]
    public void Then_It_Returns_A_Achternaam(string naam)
    {
        var voornaam = Achternaam.Create(naam);
        voornaam.Should().NotBeNull();
    }
}
