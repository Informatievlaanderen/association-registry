namespace AssociationRegistry.Test.When_Creating_A_TelefoonNummer;

using Contactgegevens.TelefoonNummers;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null
{
    [Fact]
    public void Then_it_returns_Null()
    {
        var telefoonNummer = TelefoonNummer.Create(null!);
        telefoonNummer.Should().BeNull();
    }
}
