namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_KboNummer;

using AssociationRegistry.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_Null_KboNummer
{
    [Fact]
    public void Then_it_returns_null()
    {
        var kboNummer = KboNummer.Create(null);

        kboNummer.Value.Should().BeNull();
    }
}
