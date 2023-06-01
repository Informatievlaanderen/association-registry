namespace AssociationRegistry.Test.When_Creating_A_Voornaam;

using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
