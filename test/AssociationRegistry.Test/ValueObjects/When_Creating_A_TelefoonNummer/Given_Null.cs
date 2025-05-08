namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_TelefoonNummer;

using AssociationRegistry.Vereniging.TelefoonNummers;
using FluentAssertions;
using Xunit;

public class Given_Null
{
    [Fact]
    public void Then_it_returns_Null()
    {
        var telefoonNummer = TelefoonNummer.Create(null!);
        telefoonNummer.Should().Be(TelefoonNummer.Leeg);
    }
}
