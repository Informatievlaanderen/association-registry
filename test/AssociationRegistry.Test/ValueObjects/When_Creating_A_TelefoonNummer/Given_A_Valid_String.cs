namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_TelefoonNummer;

using AssociationRegistry.Vereniging.TelefoonNummers;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Valid_String
{
    [Theory]
    [InlineData("0123456789")]
    [InlineData("0 1-2/3(4)5.6")]
    public void Then_it_creates_a_new_TelefoonNummer(string telefoonNummerString)
    {
        var telefoonNummer = TelefoonNummer.Create(telefoonNummerString);
        telefoonNummer.Waarde.Should().BeEquivalentTo(telefoonNummerString);
    }
}
