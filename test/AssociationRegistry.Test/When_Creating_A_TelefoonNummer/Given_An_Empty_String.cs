namespace AssociationRegistry.Test.When_Creating_A_TelefoonNummer;

using FluentAssertions;
using Vereniging.TelefoonNummers;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Empty_String
{
    [Fact]
    public void Then_it_returns_Null()
    {
        var telefoonNummer = TelefoonNummer.Create(string.Empty);
        telefoonNummer.Should().Be(TelefoonNummer.Leeg);
    }
}
