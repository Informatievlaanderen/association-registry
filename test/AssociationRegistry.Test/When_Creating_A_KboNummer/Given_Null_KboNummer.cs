namespace AssociationRegistry.Test.When_Creating_A_KboNummer;

using FluentAssertions;
using KboNummers;
using Xunit;

public class Given_Null_KboNummer
{
    [Fact]
    public void Then_it_returns_null()
    {
        var kboNummer = KboNummer.Create(null);

        kboNummer.Should().BeNull();
    }
}
